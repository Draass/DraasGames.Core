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

        /// <summary>
        /// Preload and cache a view without showing it.
        /// </summary>
        public async UniTask PreloadView<T>() where T : MonoBehaviour, IView
        {
            throw new NotImplementedException();

            var viewType = typeof(T);

            if (_cachedViews.ContainsKey(viewType))
            {
                Debug.LogWarning($"View {viewType} is already cached");
                return;
            }

            var view = await _viewFactory.Create<T>();
            _cachedViews.Add(viewType, view);
            view.Hide(); // Ensure the view is hidden
        }


        public void Show<T>() where T : MonoBehaviour, IView => ShowAsync<T>().Forget();

        public async UniTask ShowAsync<T>() where T : MonoBehaviour, IView
        {
            HideAllModalViews();

            Type? currentViewType = _viewStack.Count > 0 ? _viewStack.Peek() : null;
            var newView = await CreateViewAsync<T>();
            _viewStack.Push(typeof(T));

            if (currentViewType != null)
            {
                HideRegularView(currentViewType);
            }

            OnViewShown?.Invoke(newView.GetType());
        }

        public void ShowModal<T>(bool closeOtherModals = true) where T : MonoBehaviour, IView =>
            ShowModalAsync<T>(closeOtherModals).Forget();

        public async UniTask ShowModalAsync<T>(bool closeOtherModals = true) where T : MonoBehaviour, IView
        {
            if (closeOtherModals)
            {
                HideAllModalViews();
            }

            var modalView = await CreateViewAsync<T>();
            _modalViewStack.Push(modalView.GetType());
            OnViewShown?.Invoke(modalView.GetType());
        }


        public void ShowPersistent<T>() where T : MonoBehaviour, IView =>
            ShowPersistentAsync<T>().Forget();

        public async UniTask ShowPersistentAsync<T>() where T : MonoBehaviour, IView
        {
            var persistentView = await CreateViewAsync<T>();
            _persistentViewStack.Push(persistentView.GetType());
            OnViewShown?.Invoke(persistentView.GetType());
        }

        private async UniTask<IView> CreateViewAsync<T>() where T : MonoBehaviour, IView
        {
            if (_cachedViews.TryGetValue(typeof(T), out var cachedView))
            {
                _activeViews.Add(typeof(T), cachedView);
                cachedView.Show();
                
                return cachedView;
            }

            var targetView = await _viewFactory.Create<T>();

            targetView.Show();

            _activeViews.Add(typeof(T), targetView);

            return targetView;
        }

        private async UniTaskVoid CreateViewAsync(Type viewType, bool modal = false)
        {
            Debug.Log("Try to create: " + viewType);

            if (!_cachedViews.TryGetValue(viewType, out var targetView))
            {
            }

            targetView = await _viewFactory.Create(viewType);

            if (targetView == null)
                throw new InvalidOperationException($"View {viewType} is not created");

            targetView.Show();

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
                currentView.Hide();
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
                currentView.Hide();
                _activeViews.Remove(viewType);
                OnViewHidden?.Invoke(viewType);

                Destroy(currentView);
                // TODO Do not destroy the view if it's cached
            }
        }

        public void HideAllModalViews()
        {
            while (_modalViewStack.Count > 0)
            {
                var modalViewType = _modalViewStack.Pop();
                if (_activeViews.TryGetValue(modalViewType, out var modalView))
                {
                    modalView.Hide();
                    OnViewHidden?.Invoke(modalViewType);
                    _activeViews.Remove(modalViewType);

                    Destroy(modalView);
                }
            }
        }

        public void ReturnModal()
        {
            if (_modalViewStack.Count > 1)
            {
                var modalViewType = _modalViewStack.Pop();

                if (_activeViews.TryGetValue(modalViewType, out var modalView))
                {
                    modalView.Hide();
                    OnViewHidden?.Invoke(modalViewType);
                    _activeViews.Remove(modalViewType);
                }
            }
        }

        /// <summary>
        /// Hide a specific view.
        /// </summary>
        public void Hide<T>() where T : MonoBehaviour, IView
        {
            var viewType = typeof(T);

            if (!_activeViews.TryGetValue(viewType, out var view))
            {
                return;
            }
            
            view.Hide();
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

            //if(_cachedViews)

            Destroy(view);

            // todo исправить для тестов, а то ловит ошибку
            // Do not destroy the view if it is cached
        }

        private void Hide(Type viewType)
        {
            if (_activeViews.TryGetValue(viewType, out var view))
            {
                view.Hide();
                OnViewHidden?.Invoke(viewType);
                _activeViews.Remove(viewType);

                Destroy(view);
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
                currentView.Hide();
                OnViewHidden?.Invoke(currentViewType);
                _activeViews.Remove(currentViewType);
            }

            // Show previous view
            var previousViewType = _viewStack.Peek();
            if (_activeViews.TryGetValue(previousViewType, out var previousView))
            {
                previousView.Show();
                OnViewShown?.Invoke(previousViewType);
            }
            else
            {
                CreateViewAsync(previousViewType, modal: false).Forget();
            }
        }

        private void Destroy(IView view)
        {
            if (view == null)
                return;

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