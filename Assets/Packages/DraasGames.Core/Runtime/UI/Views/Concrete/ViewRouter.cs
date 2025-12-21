#nullable enable
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DraasGames.Core.Runtime.UI.Views.Abstract;
using UnityEditor;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace DraasGames.Core.Runtime.UI.Views.Concrete
{
    public class ViewRouter : IViewRouter
    {
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
        
        public event Action<Type> OnViewShown;
        
        public event Action<Type> OnViewHidden;

        public IReadOnlyDictionary<Type, IViewBase> CachedViews => _cachedViews;
        
        public IReadOnlyDictionary<Type, IViewBase> ActiveViews => _activeViews;
        
        public void Show<T>() where T : MonoBehaviour, IView => ShowAsync<T>().Forget();

        public async UniTask<T> ShowAsync<T>(ViewTransitionMode transitionMode = ViewTransitionMode.Sequential) 
            where T : MonoBehaviour, IView
        {
            await HideAllModalViewsAsync();

            ViewRouteEntry? currentEntry = _viewStack.Count > 0 ? _viewStack.Peek() : null;
            var showRequest = new ShowNoParam();

            if (transitionMode == ViewTransitionMode.Simultaneous && currentEntry != null)
            {
                // Создаем новое окно (но пока не показываем)
                var newView = await CreateViewWithoutShowAsync<T>();
                _viewStack.Push(new ViewRouteEntry(typeof(T), showRequest));

                // Запускаем анимации одновременно
                var hideTask = HideRegularViewAsync(currentEntry.ViewType);
                var showTask = showRequest.ShowAsync(newView);

                await UniTask.WhenAll(hideTask, showTask);

                OnViewShown?.Invoke(newView.GetType());
                return (T)newView;
            }

            var createdView = await CreateViewAsync<T>();
            _viewStack.Push(new ViewRouteEntry(typeof(T), showRequest));

            if (currentEntry != null)
            {
                HideRegularView(currentEntry.ViewType);
            }

            OnViewShown?.Invoke(createdView.GetType());
            return (T)createdView;
        }

        public async UniTask<T> ShowAsync<T, TParam>(TParam param, ViewTransitionMode transitionMode = ViewTransitionMode.Sequential)
            where T : MonoBehaviour, IView<TParam>
        {
            await HideAllModalViewsAsync();

            ViewRouteEntry? currentEntry = _viewStack.Count > 0 ? _viewStack.Peek() : null;
            var showRequest = new ShowParam<TParam>(param);

            if (transitionMode == ViewTransitionMode.Simultaneous && currentEntry != null)
            {
                // Создаем новое окно (но пока не показываем)
                var newView = await CreateViewWithoutShowAsync<T>();
                _viewStack.Push(new ViewRouteEntry(typeof(T), showRequest));

                // Запускаем анимации одновременно
                var hideTask = HideRegularViewAsync(currentEntry.ViewType);
                var showTask = showRequest.ShowAsync(newView);

                await UniTask.WhenAll(hideTask, showTask);

                OnViewShown?.Invoke(newView.GetType());
                return (T)newView;
            }

            var createdView = await CreateViewAsync<T, TParam>(param);
            _viewStack.Push(new ViewRouteEntry(typeof(T), showRequest));

            if (currentEntry != null)
            {
                HideRegularView(currentEntry.ViewType);
            }

            OnViewShown?.Invoke(createdView.GetType());
            return (T)createdView;
        }

        public void ShowModal<T>(bool closeOtherModals = true) where T : MonoBehaviour, IView =>
            ShowModalAsync<T>(closeOtherModals).Forget();

        public async UniTask<T> ShowModalAsync<T>(bool closeOtherModals = true) 
            where T : MonoBehaviour, IView
        {
            if (closeOtherModals)
            {
                await HideAllModalViewsAsync();
            }

            var modalView = await CreateViewAsync<T>();
            _modalViewStack.Push(modalView.GetType());
            OnViewShown?.Invoke(modalView.GetType());

            return (T)modalView;
        }

        public async UniTask<T> ShowModalAsync<T, TParam>(TParam param, bool closeOtherModals = true)
            where T : MonoBehaviour, IView<TParam>
        {
            if (closeOtherModals)
            {
                await HideAllModalViewsAsync();
            }

            var modalView = await CreateViewAsync<T, TParam>(param);
            _modalViewStack.Push(modalView.GetType());
            OnViewShown?.Invoke(modalView.GetType());

            return (T)modalView;
        }


        public void ShowPersistent<T>() where T : MonoBehaviour, IView =>
            ShowPersistentAsync<T>().Forget();

        public async UniTask<T> ShowPersistentAsync<T>() 
            where T : MonoBehaviour, IView
        {
            var persistentView = await CreateViewAsync<T>();
            _persistentViewStack.Push(persistentView.GetType());
            OnViewShown?.Invoke(persistentView.GetType());
            
            return (T)persistentView;
        }

        public async UniTask<T> ShowPersistentAsync<T, TParam>(TParam param) 
            where T : MonoBehaviour, IView<TParam>
        {
            var persistentView = await CreateViewAsync<T, TParam>(param);
            _persistentViewStack.Push(persistentView.GetType());
            OnViewShown?.Invoke(persistentView.GetType());
            
            return (T)persistentView;
        }
        
         public void ReturnModal()
        {
            if (_modalViewStack.Count > 1)
            {
                var modalViewType = _modalViewStack.Pop();

                if (_activeViews.TryGetValue(modalViewType, out var modalView))
                {
                    modalView.HideAsync().Forget();
                    OnViewHidden?.Invoke(modalViewType);
                    _activeViews.Remove(modalViewType);
                }
            }
        }

        public async UniTask ReturnModalAsync()
        {
            if (_modalViewStack.Count > 1)
            {
                var modalViewType = _modalViewStack.Pop();

                if (_activeViews.TryGetValue(modalViewType, out var modalView))
                {
                    await modalView.HideAsync();
                    OnViewHidden?.Invoke(modalViewType);
                    _activeViews.Remove(modalViewType);
                    Destroy(modalView);
                }
            }
        }

        /// <summary>
        /// Hide a specific view.
        /// </summary>
        public void Hide<T>() 
            where T : MonoBehaviour, IViewBase
        {
            HideAsync<T>().Forget();
        }

        /// <summary>
        /// Hide a specific view async.
        /// </summary>
        public async UniTask HideAsync<T>() 
            where T : MonoBehaviour, IViewBase
        {
            var viewType = typeof(T);

            if (!_activeViews.TryGetValue(viewType, out var view))
            {
                return;
            }
            
            await view.HideAsync();
            OnViewHidden?.Invoke(viewType);

            _activeViews.Remove(viewType);

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

            Destroy(view);
        }
        
        public void HideAllModalViews()
        {
            while (_modalViewStack.Count > 0)
            {
                var modalViewType = _modalViewStack.Pop();
                if (_activeViews.TryGetValue(modalViewType, out var modalView))
                {
                    modalView.HideAsync().Forget();
                    OnViewHidden?.Invoke(modalViewType);
                    _activeViews.Remove(modalViewType);

                    Destroy(modalView);
                }
            }
        }

        public async UniTask HideAllModalViewsAsync()
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
                OnViewHidden?.Invoke(viewType);
                _activeViews.Remove(viewType);
                Destroy(view);
            }
        }

        private async UniTask<IViewBase> CreateViewAsync<T>() 
            where T : MonoBehaviour, IView
        {
            if (_cachedViews.TryGetValue(typeof(T), out var cachedView))
            {
                _activeViews.Add(typeof(T), cachedView);
                await ((IView)cachedView).ShowAsync();

                return cachedView;
            }

            var targetView = await _viewFactory.Create<T>();

            await targetView.ShowAsync();

            _activeViews.Add(typeof(T), targetView);

            return targetView;
        }

        private async UniTask<IViewBase> CreateViewAsync<T, TParam>(TParam param) 
            where T : MonoBehaviour, IView<TParam>
        {
            if (_cachedViews.TryGetValue(typeof(T), out var cachedView))
            {
                _activeViews.Add(typeof(T), cachedView);
                await ((IView<TParam>)cachedView).ShowAsync(param);

                return cachedView;
            }

            var targetView = await _viewFactory.Create<T>();

            await targetView.ShowAsync(param);

            _activeViews.Add(typeof(T), targetView);

            return targetView;
        }

        private async UniTask<IViewBase> CreateViewWithoutShowAsync<T>() 
            where T : MonoBehaviour, IViewBase
        {
            if (_cachedViews.TryGetValue(typeof(T), out var cachedView))
            {
                _activeViews.Add(typeof(T), cachedView);
                return cachedView;
            }

            var targetView = await _viewFactory.Create<T>();
            _activeViews.Add(typeof(T), targetView);

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
                currentView.HideAsync().Forget();
                _activeViews.Remove(currentEntry.ViewType);
                OnViewHidden?.Invoke(currentEntry.ViewType);
                // TODO Do not destroy the view if it's cached
            }
        }

        private void HideRegularView(Type viewType)
        {
            // Hide only regular views
            if (_activeViews.TryGetValue(viewType, out var currentView) &&
                !_modalViewStack.Contains(viewType) &&
                !_persistentViewStack.Contains(viewType))
            {
                currentView.HideAsync().Forget();
                _activeViews.Remove(viewType);
                OnViewHidden?.Invoke(viewType);

                Destroy(currentView);
                // TODO Do not destroy the view if it's cached
            }
        }

        private async UniTask HideRegularViewAsync(Type viewType)
        {
            // Hide only regular views
            if (_activeViews.TryGetValue(viewType, out var currentView) &&
                !_modalViewStack.Contains(viewType) &&
                !_persistentViewStack.Contains(viewType))
            {
                await currentView.HideAsync();
                _activeViews.Remove(viewType);
                OnViewHidden?.Invoke(viewType);

                Destroy(currentView);
                // TODO Do not destroy the view if it's cached
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
            // TODO to async api, do not delete current view unless previous one exists
            
            if (closeOtherModals)
                HideAllModalViews();

            if (_viewStack.Count <= 1)
            {
                Debug.LogWarning("No previous view to return to!");
                return;
            }

            // Hide current view
            var currentEntry = _viewStack.Pop();
            if (_activeViews.TryGetValue(currentEntry.ViewType, out var currentView))
            {
                currentView.HideAsync().Forget();
                OnViewHidden?.Invoke(currentEntry.ViewType);
                _activeViews.Remove(currentEntry.ViewType);
            }

            // Show previous view
            var previousEntry = _viewStack.Peek();
            if (_activeViews.TryGetValue(previousEntry.ViewType, out var previousView))
            {
                previousEntry.ShowRequest.ShowAsync(previousView).Forget();
                OnViewShown?.Invoke(previousEntry.ViewType);
            }
            else
            {
                EnsureViewActiveAsync(previousEntry).Forget();
            }
        }

        /// <summary>
        /// Return to the previous view and close all modal views async.
        /// </summary>
        public async UniTask ReturnAsync(bool closeOtherModals = true)
        {
            if (closeOtherModals)
                await HideAllModalViewsAsync();

            if (_viewStack.Count <= 1)
            {
                Debug.LogWarning("No previous view to return to!");
                return;
            }

            // Hide current view
            var currentEntry = _viewStack.Pop();
            if (_activeViews.TryGetValue(currentEntry.ViewType, out var currentView))
            {
                await currentView.HideAsync();
                OnViewHidden?.Invoke(currentEntry.ViewType);
                _activeViews.Remove(currentEntry.ViewType);
                Destroy(currentView);
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

        private void Destroy(IViewBase view)
        {
            if (view == null)
            {
                return;
            }

#if(UNITY_EDITOR)
            if (EditorApplication.isPlaying)
            {
                Object.Destroy((view as MonoBehaviour).gameObject);
            }
            else
            {
                Object.DestroyImmediate((view as MonoBehaviour).gameObject);
            }
#else
            GameObject.Destroy((view as MonoBehaviour).gameObject);
#endif
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
