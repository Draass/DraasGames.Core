using System;
using Cysharp.Threading.Tasks;

namespace DraasGames.Core.Runtime.UI.Views.Abstract
{
    public interface IViewRouter
    {
        event Action<Type> OnViewShown;

        event Action<Type> OnViewHidden;

        /// <summary>
        /// Show a regular view, hiding the current one.
        /// </summary>
        [Obsolete("This method is obsolete and is replaced with async API. " +
                  "Consider using ShowAsync instead.")]
        void Show<T>() where T : IView;

        /// <summary>
        /// Show a regular view async, hiding the current one.
        /// </summary>
        UniTask<T> ShowAsync<T>(ViewTransitionMode transitionMode = ViewTransitionMode.Sequential)
            where T : IView;

        /// <summary>
        /// Show a regular view async with a parameter, hiding the current one.
        /// </summary>
        UniTask<T> ShowAsync<T, TParam>(TParam param, ViewTransitionMode transitionMode = ViewTransitionMode.Sequential)
            where T : IView<TParam>;

        /// <summary>
        /// Show a modal view.
        /// </summary>
        [Obsolete("This method is obsolete and is replaced with async API. " +
                  "Consider using ShowModalAsync instead.")]
        void ShowModal<T>(bool closeOtherModals = true) where T : IView;

        /// <summary>
        /// Show a modal view async.
        /// </summary>
        UniTask<T> ShowModalAsync<T>(bool closeOtherModals = true) where T : IView;

        /// <summary>
        /// Show a modal view with a parameter.
        /// </summary>
        UniTask<T> ShowModalAsync<T, TParam>(TParam param, bool closeOtherModals = true)
            where T : IView<TParam>;

        /// <summary>
        /// Show a persistent view without hiding the current one. Call Hide manually to hide it.
        /// </summary>
        [Obsolete("This method is obsolete and is replaced with async API. " +
                  "Consider using ShowPersistentAsync instead.")]
        void ShowPersistent<T>() where T : IView;

        /// <summary>
        /// Show a persistent view async.
        /// </summary>
        UniTask<T> ShowPersistentAsync<T>() where T : IView;

        /// <summary>
        /// Show a persistent view with a parameter without hiding the current one. Call Hide manually to hide it.
        /// </summary>
        UniTask<T> ShowPersistentAsync<T, TParam>(TParam param) where T : IView<TParam>;

        /// <summary>
        /// Hide a specific view.
        /// </summary>
        void Hide<T>() where T : IViewBase;

        /// <summary>
        /// Hide a specific view async.
        /// </summary>
        UniTask HideAsync<T>() where T : IViewBase;

        /// <summary>
        /// Returns true if an inactive cached instance exists for this view type.
        /// </summary>
        bool IsCached<T>() where T : IViewBase;

        /// <summary>
        /// Create a cacheable view instance before it is shown.
        /// </summary>
        UniTask PreloadAsync<T>() where T : IViewBase;

        /// <summary>
        /// Destroy a cached inactive view instance.
        /// </summary>
        bool ReleaseCached<T>() where T : IViewBase;

        /// <summary>
        /// Destroy all cached inactive view instances.
        /// </summary>
        void ReleaseAllCached();

        /// <summary>
        /// Hide all current viewed modal views.
        /// </summary>
        void HideAllModalViews();

        /// <summary>
        /// Hide all current viewed modal views async.
        /// </summary>
        UniTask HideAllModalViewsAsync();

        /// <summary>
        /// Return to the previous view and close all modal views by default.
        /// </summary>
        void Return(bool closeOtherModals = true);

        /// <summary>
        /// Return to the previous view and close all modal views by default async.
        /// </summary>
        UniTask ReturnAsync(bool closeOtherModals = true);

        /// <summary>
        /// Return to the previous modal view or close the current modal view if there is no previous modal view.
        /// </summary>
        void ReturnModal();

        /// <summary>
        /// Return to the previous modal view or close the current modal view if there is no previous modal view async.
        /// </summary>
        UniTask ReturnModalAsync();
    }
}
