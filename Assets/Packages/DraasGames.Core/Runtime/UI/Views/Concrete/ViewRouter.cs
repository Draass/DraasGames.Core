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
        public event Action<Type> OnViewShown;
        public event Action<Type> OnViewHidden;

        public IReadOnlyDictionary<Type, IView> CachedViews => _cachedViews;
        public IReadOnlyDictionary<Type, IView> ActiveViews => _activeViews;

        private readonly Dictionary<Type, IView> _cachedViews = new();
        private readonly Dictionary<Type, IView> _activeViews = new();

        private readonly Stack<Type> _viewStack = new();
        private readonly Stack<Type> _modalViewStack = new();
        private readonly Stack<Type> _persistentViewStack = new();

        private readonly IViewFactory _viewFactory;

        [Inject]
        public ViewRouter(IViewFactory viewFactory)
        {
            _viewFactory = viewFactory;
        }
        
        public void Show<T>() where T : MonoBehaviour, IView => ShowAsync<T>().Forget();

        public async UniTask<T> ShowAsync<T>() where T : MonoBehaviour, IView
        {
            await HideAllModalViewsAsync();

            Type? currentViewType = _viewStack.Count > 0 ? _viewStack.Peek() : null;
            var newView = await CreateViewAsync<T>();
            _viewStack.Push(typeof(T));

            if (currentViewType != null)
            {
                HideRegularView(currentViewType);
            }

            OnViewShown?.Invoke(newView.GetType());
            
            return (T)newView;
        }

        public void ShowModal<T>(bool closeOtherModals = true) where T : MonoBehaviour, IView =>
            ShowModalAsync<T>(closeOtherModals).Forget();

        public async UniTask<T> ShowModalAsync<T>(bool closeOtherModals = true) where T : MonoBehaviour, IView
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


        public void ShowPersistent<T>() where T : MonoBehaviour, IView =>
            ShowPersistentAsync<T>().Forget();

        public async UniTask<T> ShowPersistentAsync<T>() where T : MonoBehaviour, IView
        {
            var persistentView = await CreateViewAsync<T>();
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
        public void Hide<T>() where T : MonoBehaviour, IView
        {
            HideAsync<T>().Forget();
        }

        /// <summary>
        /// Hide a specific view async.
        /// </summary>
        public async UniTask HideAsync<T>() where T : MonoBehaviour, IView
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
            else if (_viewStack.Contains(viewType))
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
            var modalViewsToHide = new List<(Type viewType, IView view)>();

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

        private async UniTask<IView> CreateViewAsync<T>() where T : MonoBehaviour, IView
        {
            if (_cachedViews.TryGetValue(typeof(T), out var cachedView))
            {
                _activeViews.Add(typeof(T), cachedView);
                await cachedView.ShowAsync();
                
                return cachedView;
            }

            var targetView = await _viewFactory.Create<T>();

            await targetView.ShowAsync();

            _activeViews.Add(typeof(T), targetView);

            return targetView;
        }

        private async UniTask CreateViewAsync(Type viewType, bool modal = false)
        {
            Debug.Log("Try to create: " + viewType);

            if (!_cachedViews.TryGetValue(viewType, out var targetView))
            {
            }

            targetView = await _viewFactory.Create(viewType);

            if (targetView == null)
                throw new InvalidOperationException($"View {viewType} is not created");

            await targetView.ShowAsync();

            if (modal)
            {
                _modalViewStack.Push(viewType);
            }
            else
            {
                _viewStack.Push(viewType);
            }

            _activeViews[viewType] = targetView;

            OnViewShown?.Invoke(viewType);
        }

        private void HideCurrentView()
        {
            if (_viewStack.Count == 0)
                return;

            var currentViewType = _viewStack.Pop();
            Debug.Log("Hiding view: " + currentViewType);

            // Hide only regular views
            if (_activeViews.TryGetValue(currentViewType, out var currentView) &&
                !_modalViewStack.Contains(currentViewType) &&
                !_persistentViewStack.Contains(currentViewType))
            {
                currentView.HideAsync().Forget();
                _activeViews.Remove(currentViewType);
                OnViewHidden?.Invoke(currentViewType);
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
            var currentViewType = _viewStack.Pop();
            if (_activeViews.TryGetValue(currentViewType, out var currentView))
            {
                currentView.HideAsync().Forget();
                OnViewHidden?.Invoke(currentViewType);
                _activeViews.Remove(currentViewType);
            }

            // Show previous view
            var previousViewType = _viewStack.Peek();
            if (_activeViews.TryGetValue(previousViewType, out var previousView))
            {
                previousView.ShowAsync().Forget();
                OnViewShown?.Invoke(previousViewType);
            }
            else
            {
                CreateViewAsync(previousViewType, modal: false).Forget();
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
            var currentViewType = _viewStack.Pop();
            if (_activeViews.TryGetValue(currentViewType, out var currentView))
            {
                await currentView.HideAsync();
                OnViewHidden?.Invoke(currentViewType);
                _activeViews.Remove(currentViewType);
                Destroy(currentView);
            }

            // Show previous view
            var previousViewType = _viewStack.Peek();
            if (_activeViews.TryGetValue(previousViewType, out var previousView))
            {
                await previousView.ShowAsync();
                OnViewShown?.Invoke(previousViewType);
            }
            else
            {
                await CreateViewAsync(previousViewType, modal: false);
            }
        }

        private void Destroy(IView view)
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
}