using Cysharp.Threading.Tasks;
using UnityEngine;

namespace DraasGames.Core.Runtime.UI.Views.Abstract
{
    public interface IViewRouter
    {
        /// <summary>
        /// Show a regular view, hiding the current one.
        /// </summary>
        public void Show<T>() where T : MonoBehaviour, IView;

        /// <summary>
        /// Show a regular view async, hiding the current one.
        /// </summary>
        public UniTask<T> ShowAsync<T>() where T : MonoBehaviour, IView;
        
        /// <summary>
        /// Show a modal view without hiding the current one.
        /// </summary>
        public void ShowModal<T>(bool closeOtherModals = true) where T : MonoBehaviour, IView;
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public UniTask<T> ShowModalAsync<T>(bool closeOtherModals = true) where T : MonoBehaviour, IView;
        
        /// <summary>
        /// Show a persistent view without hiding the current one. Call Hide manually to hide it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void ShowPersistent<T>() where T : MonoBehaviour, IView;
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public UniTask<T> ShowPersistentAsync<T>() where T : MonoBehaviour, IView;
        
        /// <summary>
        /// Hide a specific view.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void Hide<T>() where T : MonoBehaviour, IView;
        
        /// <summary>
        /// Hide all current viewed modal views
        /// </summary>
        public void HideAllModalViews();
        
        /// <summary>
        /// Return to the previous view and close all modal views by default.
        /// </summary>
        public void Return(bool closeOtherModals = true);
        
        /// <summary>
        /// Return to the previous modal view or close the current modal view if there is no previous modal view.
        /// </summary>
        public void ReturnModal();
    }
}