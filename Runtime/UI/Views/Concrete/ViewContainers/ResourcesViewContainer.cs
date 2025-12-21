using System;
using System.Collections.Generic;
using DraasGames.Core.Runtime.UI.Views.Abstract;
using DraasGames.Core.Runtime.UI.Views.Concrete.ViewProviders;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DraasGames.Core.Runtime.UI.Views.Concrete.ViewContainers
{
    [CreateAssetMenu(menuName = "DraasGames/UI/ViewContainer", fileName = "View Container")]
    public class ResourcesViewContainer : SerializedScriptableObject
    {
        [SerializeField, BoxGroup("Add View")]
        private IViewBase _viewToAdd;

        [SerializeField]
        private Dictionary<Type, string> _viewPathsPair = new();

#if UNITY_EDITOR
        private IViewPathRetrieveStrategy _pathRetrieveStrategy = new ResourcesViewPathRetrieveStrategy();
#endif

        public string GetViewPath<T>()
        {
            return _viewPathsPair[typeof(T)];
        }

        public string GetViewPath(Type viewType)
        {
            if (!viewType.IsAssignableFrom(typeof(MonoBehaviour)))
                throw new ArgumentException($"Type {viewType} is not a MonoBehaviour");

            if (!viewType.IsAssignableFrom(typeof(IViewBase)))
                throw new ArgumentException($"Type {viewType} is not an IViewBase");

            return _viewPathsPair[viewType];
        }

#if UNITY_EDITOR
        [Button, BoxGroup("Add View")]
        private void EditorAddView()
        {
            AddView(_viewToAdd);
        }

        public void AddView(IViewBase view)
        {
            Type derivedType = view.GetType();

            var path = _pathRetrieveStrategy.RetrieveViewPath(view);

            if (_viewPathsPair.TryGetValue(derivedType, out var viewPath))
            {
                _viewPathsPair[derivedType] = path;
            }
            else
            {
                _viewPathsPair.Add(derivedType, path);
            }
        }
#endif
    }
}
