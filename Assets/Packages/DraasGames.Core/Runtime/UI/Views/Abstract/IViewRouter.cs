using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace DraasGames.Core.Runtime.UI.Views.Abstract
{
    public interface IViewRouter
    {
        /// <summary>
        /// Show a regular view, hiding the current one.
        /// </summary>
        [Obsolete("This method is obsolete and is replaced with async API. " +
                  "Consider using ShowAsync instead.")]
        void Show<T>() where T : MonoBehaviour, IView;

        /// <summary>
        /// Show a regular view async, hiding the current one.
        /// </summary>
        UniTask<T> ShowAsync<T>(ViewTransitionMode transitionMode = ViewTransitionMode.Sequential)
            where T : MonoBehaviour, IView;

        /// <summary>
        /// Show a regular view async with a parameter, hiding the current one.
        /// </summary>
        UniTask<T> ShowAsync<T, TParam>(TParam param, ViewTransitionMode transitionMode = ViewTransitionMode.Sequential)
            where T : MonoBehaviour, IView<TParam>;

        /// <summary>
        /// Show a modal view.
        /// </summary>
        [Obsolete("This method is obsolete and is replaced with async API. " +
                  "Consider using ShowModalAsync instead.")]
        void ShowModal<T>(bool closeOtherModals = true) where T : MonoBehaviour, IView;

        /// <summary>
        /// Show a modal view async.
        /// </summary>
        UniTask<T> ShowModalAsync<T>(bool closeOtherModals = true) where T : MonoBehaviour, IView;

        /// <summary>
        /// Show a modal view with a parameter.
        /// </summary>
        UniTask<T> ShowModalAsync<T, TParam>(TParam param, bool closeOtherModals = true)
            where T : MonoBehaviour, IView<TParam>;

        /// <summary>
        /// Show a persistent view without hiding the current one. Call Hide manually to hide it.
        /// </summary>
        [Obsolete("This method is obsolete and is replaced with async API. " +
                  "Consider using ShowPersistentAsync instead.")]
        void ShowPersistent<T>() where T : MonoBehaviour, IView;

        /// <summary>
        /// Show a persistent view async.
        /// </summary>
        UniTask<T> ShowPersistentAsync<T>() where T : MonoBehaviour, IView;

        /// <summary>
        /// Show a persistent view with a parameter without hiding the current one. Call Hide manually to hide it.
        /// </summary>
        UniTask<T> ShowPersistentAsync<T, TParam>(TParam param) where T : MonoBehaviour, IView<TParam>;

        /// <summary>
        /// Hide a specific view.
        /// </summary>
        void Hide<T>() where T : MonoBehaviour, IViewBase;

        /// <summary>
        /// Hide a specific view async.
        /// </summary>
        UniTask HideAsync<T>() where T : MonoBehaviour, IViewBase;

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
