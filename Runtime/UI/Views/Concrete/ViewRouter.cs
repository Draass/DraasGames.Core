#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DraasGames.Core.Runtime.UI.Views.Abstract;
using UnityEngine;

namespace DraasGames.Core.Runtime.UI.Views.Concrete
{
    public class ViewRouter : IViewRouter
    {
        private readonly SemaphoreSlim _transitionLock = new(1, 1);

        private readonly Dictionary<Type, IViewBase> _cachedViews = new();
        private readonly Dictionary<Type, IViewBase> _activeViews = new();

        private readonly Stack<ViewRouteEntry> _viewStack = new();
        private readonly Stack<Type> _modalViewStack = new();
        private readonly Stack<Type> _persistentViewStack = new();

        private readonly IViewFactory _viewFactory;

        public ViewRouter(IViewFactory viewFactory)
        {
            _viewFactory = viewFactory;
        }

        private async UniTask<IDisposable> EnterTransitionLockAsync()
        {
            await _transitionLock.WaitAsync();
            return new TransitionLockReleaser(_transitionLock);
        }

        private readonly struct TransitionLockReleaser : IDisposable
        {
            private readonly SemaphoreSlim _semaphore;

            public TransitionLockReleaser(SemaphoreSlim semaphore)
            {
                _semaphore = semaphore;
            }

            public void Dispose()
            {
                _semaphore.Release();
            }
        }
        
        public event Action<Type>? OnViewShown;
        
        public event Action<Type>? OnViewHidden;

        public IReadOnlyDictionary<Type, IViewBase> CachedViews => _cachedViews;
        
        public IReadOnlyDictionary<Type, IViewBase> ActiveViews => _activeViews;

        public bool IsCached<T>() where T : IViewBase
        {
            return _cachedViews.ContainsKey(typeof(T));
        }

        public async UniTask PreloadAsync<T>() where T : IViewBase
        {
            using (await EnterTransitionLockAsync())
            {
                var viewType = typeof(T);
                if (_activeViews.ContainsKey(viewType) || _cachedViews.ContainsKey(viewType))
                {
                    return;
                }

                var view = await _viewFactory.Create<T>();
                if (view == null)
                {
                    throw new InvalidOperationException($"View {viewType} is not created");
                }

                if (view is not ICacheableView cacheableView)
                {
                    Destroy(view);
                    throw new InvalidOperationException(
                        $"View {viewType} must implement {nameof(ICacheableView)} to be preloaded.");
                }

                await view.HideAsync();
                await cacheableView.ResetStateAsync();
                _cachedViews[viewType] = view;
            }
        }

        public bool ReleaseCached<T>() where T : IViewBase
        {
            return ReleaseCached(typeof(T));
        }

        public void ReleaseAllCached()
        {
            var cachedViewTypes = new List<Type>(_cachedViews.Keys);
            foreach (var viewType in cachedViewTypes)
            {
                ReleaseCached(viewType);
            }
        }
        
        public void Show<T>() where T : IView => ShowAsync<T>().Forget();

        public async UniTask<T> ShowAsync<T>(ViewTransitionMode transitionMode = ViewTransitionMode.Sequential) 
            where T : IView
        {
            using (await EnterTransitionLockAsync())
            {
                await HideAllModalViewsInternalAsync();

                var targetViewType = typeof(T);

                // Prevent duplicates when the same view is requested repeatedly (e.g. spam clicking a button).
                if (_viewStack.Count > 0 &&
                    _viewStack.Peek().ViewType == targetViewType &&
                    _activeViews.TryGetValue(targetViewType, out var alreadyActiveView))
                {
                    return (T)alreadyActiveView;
                }

                ViewRouteEntry? currentEntry = _viewStack.Count > 0 ? _viewStack.Peek() : null;
                var showRequest = new ShowNoParam();

                if (transitionMode == ViewTransitionMode.Simultaneous && currentEntry != null)
                {
                    var newView = await CreateViewWithoutShowAsync<T>();
                    _viewStack.Push(new ViewRouteEntry(targetViewType, showRequest));

                    var hideTask = HideRegularViewAsync(currentEntry.ViewType);
                    var showTask = showRequest.ShowAsync(newView);

                    await UniTask.WhenAll(hideTask, showTask);

                    OnViewShown?.Invoke(newView.GetType());
                    return (T)newView;
                }

                var createdView = await CreateViewAsync<T>();
                _viewStack.Push(new ViewRouteEntry(targetViewType, showRequest));

                if (currentEntry != null)
                {
                    await HideRegularViewAsync(currentEntry.ViewType);
                }

                OnViewShown?.Invoke(createdView.GetType());
                return (T)createdView;
            }
        }

        public async UniTask<T> ShowAsync<T, TParam>(TParam param, ViewTransitionMode transitionMode = ViewTransitionMode.Sequential)
            where T : IView<TParam>
        {
            using (await EnterTransitionLockAsync())
            {
                await HideAllModalViewsInternalAsync();

                var targetViewType = typeof(T);

                // Prevent duplicates when the same view is requested repeatedly (e.g. spam clicking a button).
                if (_viewStack.Count > 0 &&
                    _viewStack.Peek().ViewType == targetViewType &&
                    _activeViews.TryGetValue(targetViewType, out var alreadyActiveView))
                {
                    return (T)alreadyActiveView;
                }

                ViewRouteEntry? currentEntry = _viewStack.Count > 0 ? _viewStack.Peek() : null;
                var showRequest = new ShowParam<TParam>(param);

                if (transitionMode == ViewTransitionMode.Simultaneous && currentEntry != null)
                {
                    // Создаем новое окно (но пока не показываем)
                    var newView = await CreateViewWithoutShowAsync<T>();
                    _viewStack.Push(new ViewRouteEntry(targetViewType, showRequest));

                    // Запускаем анимации одновременно
                    var hideTask = HideRegularViewAsync(currentEntry.ViewType);
                    var showTask = showRequest.ShowAsync(newView);

                    await UniTask.WhenAll(hideTask, showTask);

                    OnViewShown?.Invoke(newView.GetType());
                    return (T)newView;
                }

                var createdView = await CreateViewAsync<T, TParam>(param);
                _viewStack.Push(new ViewRouteEntry(targetViewType, showRequest));

                if (currentEntry != null)
                {
                    await HideRegularViewAsync(currentEntry.ViewType);
                }

                OnViewShown?.Invoke(createdView.GetType());
                return (T)createdView;
            }
        }

        public void ShowModal<T>(bool closeOtherModals = true) where T : IView =>
            ShowModalAsync<T>(closeOtherModals).Forget();

        public async UniTask<T> ShowModalAsync<T>(bool closeOtherModals = true) 
            where T : IView
        {
            using (await EnterTransitionLockAsync())
            {
                if (closeOtherModals)
                {
                    await HideAllModalViewsInternalAsync();
                }

                var viewType = typeof(T);
                if (_activeViews.TryGetValue(viewType, out var alreadyActive) && _modalViewStack.Contains(viewType))
                {
                    return (T)alreadyActive;
                }

                var modalView = await CreateViewAsync<T>();
                _modalViewStack.Push(modalView.GetType());
                OnViewShown?.Invoke(modalView.GetType());

                return (T)modalView;
            }
        }

        public async UniTask<T> ShowModalAsync<T, TParam>(TParam param, bool closeOtherModals = true)
            where T : IView<TParam>
        {
            using (await EnterTransitionLockAsync())
            {
                if (closeOtherModals)
                {
                    await HideAllModalViewsInternalAsync();
                }

                var viewType = typeof(T);
                if (_activeViews.TryGetValue(viewType, out var alreadyActive) && _modalViewStack.Contains(viewType))
                {
                    return (T)alreadyActive;
                }

                var modalView = await CreateViewAsync<T, TParam>(param);
                _modalViewStack.Push(modalView.GetType());
                OnViewShown?.Invoke(modalView.GetType());

                return (T)modalView;
            }
        }


        public void ShowPersistent<T>() where T : IView =>
            ShowPersistentAsync<T>().Forget();

        public async UniTask<T> ShowPersistentAsync<T>() 
            where T : IView
        {
            using (await EnterTransitionLockAsync())
            {
                var viewType = typeof(T);
                if (_activeViews.TryGetValue(viewType, out var alreadyActive) && _persistentViewStack.Contains(viewType))
                {
                    return (T)alreadyActive;
                }

                var persistentView = await CreateViewAsync<T>();
                _persistentViewStack.Push(persistentView.GetType());
                OnViewShown?.Invoke(persistentView.GetType());
                
                return (T)persistentView;
            }
        }

        public async UniTask<T> ShowPersistentAsync<T, TParam>(TParam param) 
            where T : IView<TParam>
        {
            using (await EnterTransitionLockAsync())
            {
                var viewType = typeof(T);
                if (_activeViews.TryGetValue(viewType, out var alreadyActive) && _persistentViewStack.Contains(viewType))
                {
                    return (T)alreadyActive;
                }

                var persistentView = await CreateViewAsync<T, TParam>(param);
                _persistentViewStack.Push(persistentView.GetType());
                OnViewShown?.Invoke(persistentView.GetType());
                
                return (T)persistentView;
            }
        }
        
         public void ReturnModal()
        {
            ReturnModalAsync().Forget();
        }

        public async UniTask ReturnModalAsync()
        {
            using (await EnterTransitionLockAsync())
            {
                if (_modalViewStack.Count > 1)
                {
                    var modalViewType = _modalViewStack.Pop();

                    if (_activeViews.TryGetValue(modalViewType, out var modalView))
                    {
                        await HideAndReleaseViewAsync(modalViewType, modalView);
                    }
                }
            }
        }

        /// <summary>
        /// Hide a specific view.
        /// </summary>
        public void Hide<T>() 
            where T : IViewBase
        {
            HideAsync<T>().Forget();
        }

        /// <summary>
        /// Hide a specific view async.
        /// </summary>
        public async UniTask HideAsync<T>() 
            where T : IViewBase
        {
            using (await EnterTransitionLockAsync())
            {
                var viewType = typeof(T);

                if (!_activeViews.TryGetValue(viewType, out var view))
                {
                    return;
                }
                
                // Remove from the correct stack
                if (_modalViewStack.Contains(viewType))
                {
                    RemoveViewFromStack(_modalViewStack, viewType);
                }
                else if (ContainsView(_viewStack, viewType))
                {
                    RemoveViewFromStack(_viewStack, viewType);
                }
                else if (_persistentViewStack.Contains(viewType))
                {
                    RemoveViewFromStack(_persistentViewStack, viewType);
                }

                await HideAndReleaseViewAsync(viewType, view);
            }
        }
        
        public void HideAllModalViews()
        {
            HideAllModalViewsAsync().Forget();
        }

        public async UniTask HideAllModalViewsAsync()
        {
            using (await EnterTransitionLockAsync())
            {
                await HideAllModalViewsInternalAsync();
            }
        }

        private async UniTask HideAllModalViewsInternalAsync()
        {
            var hideTasks = new List<UniTask>();
            var modalViewsToHide = new List<(Type viewType, IViewBase view)>();

            while (_modalViewStack.Count > 0)
            {
                var modalViewType = _modalViewStack.Pop();
                if (_activeViews.TryGetValue(modalViewType, out var modalView))
                {
                    modalViewsToHide.Add((modalViewType, modalView));
                    hideTasks.Add(modalView.HideAsync());
                }
            }

            await UniTask.WhenAll(hideTasks);

            foreach (var (viewType, view) in modalViewsToHide)
            {
                ReleaseHiddenView(viewType, view);
            }
        }

        private async UniTask<IViewBase> CreateViewAsync<T>() 
            where T : IView
        {
            var viewType = typeof(T);
            if (_activeViews.TryGetValue(viewType, out var alreadyActive))
            {
                return alreadyActive;
            }

            if (_cachedViews.TryGetValue(viewType, out var cachedView))
            {
                _cachedViews.Remove(viewType);
                await ResetCachedViewAsync(cachedView);
                _activeViews[viewType] = cachedView;
                await ((IView)cachedView).ShowAsync();
                return cachedView;
            }

            var targetView = await _viewFactory.Create<T>();
            if (targetView == null)
                throw new InvalidOperationException($"View {viewType} is not created");

            _activeViews[viewType] = targetView;
            await targetView.ShowAsync();
            return targetView;
        }

        private async UniTask<IViewBase> CreateViewAsync<T, TParam>(TParam param) 
            where T : IView<TParam>
        {
            var viewType = typeof(T);
            if (_activeViews.TryGetValue(viewType, out var alreadyActive))
            {
                return alreadyActive;
            }

            if (_cachedViews.TryGetValue(viewType, out var cachedView))
            {
                _cachedViews.Remove(viewType);
                await ResetCachedViewAsync(cachedView);
                _activeViews[viewType] = cachedView;
                await ((IView<TParam>)cachedView).ShowAsync(param);
                return cachedView;
            }

            var targetView = await _viewFactory.Create<T>();
            if (targetView == null)
                throw new InvalidOperationException($"View {viewType} is not created");

            _activeViews[viewType] = targetView;
            await targetView.ShowAsync(param);
            return targetView;
        }

        private async UniTask<IViewBase> CreateViewWithoutShowAsync<T>() 
            where T : IViewBase
        {
            var viewType = typeof(T);
            if (_activeViews.TryGetValue(viewType, out var alreadyActive))
            {
                return alreadyActive;
            }

            if (_cachedViews.TryGetValue(viewType, out var cachedView))
            {
                _cachedViews.Remove(viewType);
                await ResetCachedViewAsync(cachedView);
                _activeViews[viewType] = cachedView;
                return cachedView;
            }

            var targetView = await _viewFactory.Create<T>();
            if (targetView == null)
                throw new InvalidOperationException($"View {viewType} is not created");

            _activeViews[viewType] = targetView;
            return targetView;
        }

        private async UniTask<IViewBase> EnsureViewActiveAsync(ViewRouteEntry entry)
        {
            if (_activeViews.TryGetValue(entry.ViewType, out var activeView))
            {
                await entry.ShowRequest.ShowAsync(activeView);
                OnViewShown?.Invoke(entry.ViewType);
                return activeView;
            }

            if (_cachedViews.TryGetValue(entry.ViewType, out var cachedView))
            {
                _cachedViews.Remove(entry.ViewType);
                await ResetCachedViewAsync(cachedView);
                _activeViews[entry.ViewType] = cachedView;
                await entry.ShowRequest.ShowAsync(cachedView);
                OnViewShown?.Invoke(entry.ViewType);
                return cachedView;
            }

            var createdView = await _viewFactory.Create(entry.ViewType);

            if (createdView == null)
                throw new InvalidOperationException($"View {entry.ViewType} is not created");

            _activeViews[entry.ViewType] = createdView;
            await entry.ShowRequest.ShowAsync(createdView);
            OnViewShown?.Invoke(entry.ViewType);

            return createdView;
        }

        private void HideCurrentView()
        {
            if (_viewStack.Count == 0)
                return;

            var currentEntry = _viewStack.Pop();
            var currentViewType = currentEntry.ViewType;
            Debug.Log("Hiding view: " + currentViewType);

            // Hide only regular views
            if (_activeViews.TryGetValue(currentViewType, out var currentView) &&
                !_modalViewStack.Contains(currentViewType) &&
                !_persistentViewStack.Contains(currentViewType))
            {
                HideAndReleaseViewAsync(currentEntry.ViewType, currentView).Forget();
            }
        }

        private void HideRegularView(Type viewType)
        {
            // Hide only regular views
            if (_activeViews.TryGetValue(viewType, out var currentView) &&
                !_modalViewStack.Contains(viewType) &&
                !_persistentViewStack.Contains(viewType))
            {
                HideAndReleaseViewAsync(viewType, currentView).Forget();
            }
        }

        private async UniTask HideAndReleaseViewAsync(Type viewType, IViewBase view)
        {
            if (view == null)
            {
                return;
            }

            await view.HideAsync();
            ReleaseHiddenView(viewType, view);
        }

        private async UniTask HideRegularViewAsync(Type viewType)
        {
            // Hide only regular views
            if (_activeViews.TryGetValue(viewType, out var currentView) &&
                !_modalViewStack.Contains(viewType) &&
                !_persistentViewStack.Contains(viewType))
            {
                await HideAndReleaseViewAsync(viewType, currentView);
            }
        }

        private void ReleaseHiddenView(Type viewType, IViewBase view)
        {
            _activeViews.Remove(viewType);
            OnViewHidden?.Invoke(viewType);

            if (view is ICacheableView)
            {
                _cachedViews[viewType] = view;
                return;
            }

            Destroy(view);
        }

        private async UniTask ResetCachedViewAsync(IViewBase view)
        {
            if (view is ICacheableView cacheableView)
            {
                await cacheableView.ResetStateAsync();
            }
        }

        private static bool ContainsView(Stack<ViewRouteEntry> stack, Type viewType)
        {
            foreach (var entry in stack)
            {
                if (entry.ViewType == viewType)
                {
                    return true;
                }
            }

            return false;
        }

        private void RemoveViewFromStack(Stack<ViewRouteEntry> stack, Type viewType)
        {
            var tempStack = new Stack<ViewRouteEntry>();
            while (stack.Count > 0)
            {
                var entry = stack.Pop();
                if (entry.ViewType != viewType)
                {
                    tempStack.Push(entry);
                }
            }

            while (tempStack.Count > 0)
            {
                stack.Push(tempStack.Pop());
            }
        }

        private void RemoveViewFromStack(Stack<Type> stack, Type viewType)
        {
            var tempStack = new Stack<Type>();
            while (stack.Count > 0)
            {
                var vType = stack.Pop();
                if (vType != viewType)
                    tempStack.Push(vType);
            }

            while (tempStack.Count > 0)
            {
                stack.Push(tempStack.Pop());
            }
        }

        /// <summary>
        /// Return to the previous view and close all modal views.
        /// </summary>
        public void Return(bool closeOtherModals = true)
        {
            ReturnAsync(closeOtherModals).Forget();
        }

        /// <summary>
        /// Return to the previous view and close all modal views async.
        /// </summary>
        public async UniTask ReturnAsync(bool closeOtherModals = true)
        {
            using (await EnterTransitionLockAsync())
            {
                if (closeOtherModals)
                {
                    await HideAllModalViewsInternalAsync();
                }

                if (_viewStack.Count <= 1)
                {
                    Debug.LogWarning("No previous view to return to!");
                    return;
                }

                // Hide current view
                var currentEntry = _viewStack.Pop();
                if (_activeViews.TryGetValue(currentEntry.ViewType, out var currentView))
                {
                    await HideAndReleaseViewAsync(currentEntry.ViewType, currentView);
                }

                // Show previous view
                var previousEntry = _viewStack.Peek();
                if (_activeViews.TryGetValue(previousEntry.ViewType, out var previousView))
                {
                    await previousEntry.ShowRequest.ShowAsync(previousView);
                    OnViewShown?.Invoke(previousEntry.ViewType);
                }
                else
                {
                    await EnsureViewActiveAsync(previousEntry);
                }
            }
        }

        private bool ReleaseCached(Type viewType)
        {
            if (_activeViews.ContainsKey(viewType))
            {
                return false;
            }

            if (!_cachedViews.Remove(viewType, out var cachedView))
            {
                return false;
            }

            Destroy(cachedView);
            return true;
        }

        private void Destroy(IViewBase view)
        {
            if (view == null)
            {
                return;
            }

            if (view is not IDestroyableView destroyableView)
            {
                throw new InvalidOperationException(
                    $"View {view.GetType()} does not implement {nameof(IDestroyableView)}.");
            }

            destroyableView.DestroyView();
        }
    }
    
    internal interface IShowRequest
    {
        UniTask ShowAsync(IViewBase view);
    }

    internal sealed record ShowNoParam : IShowRequest
    {
        public UniTask ShowAsync(IViewBase view)
        {
            return ((IView)view).ShowAsync();
        }
    }

    internal sealed record ShowParam<TParam> : IShowRequest
    {
        private readonly TParam _param;

        public ShowParam(TParam param)
        {
            _param = param;
        }

        public UniTask ShowAsync(IViewBase view)
        {
            return ((IView<TParam>)view).ShowAsync(_param);
        }
    }

    internal sealed record ViewRouteEntry
    {
        public Type ViewType { get; }
        public IShowRequest ShowRequest { get; }

        public ViewRouteEntry(Type viewType, IShowRequest showRequest)
        {
            ViewType = viewType;
            ShowRequest = showRequest;
        }
    }
}
