using System;
using DraasGames.Core.Runtime.UI.Views.Abstract;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace DraasGames.Core.Runtime.UI.Views.Concrete
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))]
    [DisallowMultipleComponent]
    public class View : MonoBehaviour, IView
    {
        protected Canvas Canvas { get; set; }
        protected GraphicRaycaster Raycaster { get; set; }

        public event Action OnViewShow;
        public event Action OnViewHide;

        protected virtual void Awake()
        {
            Canvas = GetComponent<Canvas>();
            Raycaster = GetComponent<GraphicRaycaster>();
        }

        [Button]
        public virtual void Show()
        {
#if UNITY_EDITOR
            Canvas = GetComponent<Canvas>();
            Raycaster = GetComponent<GraphicRaycaster>();
#endif
            
            Raycaster.enabled = true;
            Canvas.enabled = true;
            OnViewShow?.Invoke();
        }

        [Button]
        public virtual void Hide()
        {
#if UNITY_EDITOR
            Canvas = GetComponent<Canvas>();
            Raycaster = GetComponent<GraphicRaycaster>();
#endif
            
            Canvas.enabled = false;
            Raycaster.enabled = true;
            OnViewHide?.Invoke();
        }
    }
}