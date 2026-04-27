using System;
using Cysharp.Threading.Tasks;
using DraasGames.Core.Runtime.UI.Views.Abstract;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DraasGames.Core.Runtime.UI.Views.Concrete
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))]
    [DisallowMultipleComponent]
    public abstract class ViewBase : MonoBehaviour, IViewBase, IDestroyableView
    {
        protected Canvas Canvas { get; private set; }
        protected GraphicRaycaster Raycaster { get; private set; }

        public event Action OnViewShow;
        public event Action OnViewHide;

        protected virtual void Awake()
        {
            CacheComponents();
        }

        protected void CacheComponents()
        {
            if (Canvas == null)
            {
                Canvas = GetComponent<Canvas>();
            }

            if (Raycaster == null)
            {
                Raycaster = GetComponent<GraphicRaycaster>();
            }
        }

        protected void ShowInternal()
        {
#if UNITY_EDITOR
            CacheComponents();
#endif
            Raycaster.enabled = true;
            Canvas.enabled = true;
            OnViewShow?.Invoke();
        }

        protected void HideInternal()
        {
#if UNITY_EDITOR
            CacheComponents();
#endif
            Canvas.enabled = false;
            Raycaster.enabled = false;
            OnViewHide?.Invoke();
        }

        public virtual UniTask HideAsync()
        {
            HideInternal();
            return UniTask.CompletedTask;
        }

        public virtual void DestroyView()
        {
#if UNITY_EDITOR
            if (EditorApplication.isPlaying)
            {
                Object.Destroy(gameObject);
            }
            else
            {
                Object.DestroyImmediate(gameObject);
            }
#else
            Object.Destroy(gameObject);
#endif
        }
    }

    public class View : ViewBase, IView
    {
        [Button]
        public virtual void Show()
        {
            ShowInternal();
        }

        public virtual UniTask ShowAsync()
        {
            ShowInternal();
            return UniTask.CompletedTask;
        }

        [Button]
        public virtual void Hide()
        {
            HideInternal();
        }
    }

    public class View<TParam> : ViewBase, IView<TParam>
    {
        public virtual UniTask ShowAsync(TParam param)
        {
            ShowInternal();
            return UniTask.CompletedTask;
        }
    }
}
