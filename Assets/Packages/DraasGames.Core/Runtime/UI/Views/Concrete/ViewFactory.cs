﻿using System;
using Cysharp.Threading.Tasks;
using DraasGames.Core.Runtime.UI.Views.Abstract;
using UnityEngine;
using Zenject;

namespace DraasGames.Core.Runtime.UI.Views.Concrete
{
    public class ViewFactory : IViewFactory
    {
        private readonly IInstantiator _instantiator;
        private readonly IViewProvider _viewProviderAsync;
        
        [Inject]
        public ViewFactory(
            IInstantiator instantiator, 
            IViewProvider viewProviderAsync)
        {
            _instantiator = instantiator;
            _viewProviderAsync = viewProviderAsync;
        }
        
        public async UniTask<IView> Create(Type viewType)
        {
            if(!typeof(IView).IsAssignableFrom(viewType))
                throw new ArgumentException($"Type {viewType} is not an IView");
            
            if(!typeof(MonoBehaviour).IsAssignableFrom(viewType))
                throw new ArgumentException($"Type {viewType} is not a MonoBehaviour");
            
            var viewPrefab = await _viewProviderAsync.GetViewAsync(viewType) as MonoBehaviour;

            return _instantiator.InstantiatePrefabForComponent<IView>(viewPrefab);
        }

        public async UniTask<T> Create<T>() where T : MonoBehaviour, IView
        {
            var viewPrefab = await _viewProviderAsync.GetViewAsync<T>() as MonoBehaviour;
            
            return _instantiator.InstantiatePrefabForComponent<T>(viewPrefab);
        }
    }
}