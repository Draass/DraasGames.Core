using System;
using System.Collections.Generic;
using DraasGames.Core.Runtime.UI.Views.Abstract;
using DraasGames.Core.Runtime.UI.Views.Concrete.ViewProviders;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DraasGames.Core.Runtime.UI.Views.Concrete.ViewContainers
{
    [CreateAssetMenu(menuName = "DraasGames/UI/ViewContainer", fileName = "View Container")]
    public class ResourcesViewContainer : ViewContainer
    {
        [SerializeField, BoxGroup("Add View")]
        private IView _viewToAdd;
        
        [SerializeField]
        private Dictionary<Type, string> _viewPathsPair = new Dictionary<Type, string>();

        private IViewPathRetrieveStrategy _pathRetrieveStrategy = new ResourcesViewPathRetrieveStrategy();
         
        public override string GetViewPath<T>()
        {
            return _viewPathsPair[typeof(T)];
        }
        
        public override string GetViewPath(Type viewType)
        {
            if (!viewType.IsAssignableFrom(typeof(MonoBehaviour)))
                throw new ArgumentException($"Type {viewType} is not a MonoBehaviour");
            
            if(!viewType.IsAssignableFrom(typeof(IView)))
                throw new ArgumentException($"Type {viewType} is not an IView");
            
            return _viewPathsPair[viewType];
        }
        
        [Button, BoxGroup("Add View")]
        private void EditorAddView()
        {
            AddView(_viewToAdd);
        }
        
        public override void AddView(IView view)
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
    }
}