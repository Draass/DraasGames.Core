using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DraasGames.Core.Runtime.Infrastructure.Loaders.Abstract;
using DraasGames.Core.Runtime.Infrastructure.Loaders.Concrete;
using DraasGames.Core.Runtime.UI.Views.Abstract;
using DraasGames.Core.Runtime.UI.Views.Concrete;
using DraasGames.Core.Runtime.UI.Views.Concrete.ViewContainers;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DraasGames.Core.Runtime.Infrastructure.Installers
{
    public class AddressablesViewProvider : IViewProvider
    {
        private readonly IAssetLoader _assetLoader;
        private readonly IViewAssetReferenceProvider _assetReferenceProvider;
        private readonly IScopeLifetimeProvider _scopeLifetimeProvider;

        public AddressablesViewProvider(
            IAssetLoader assetLoader, 
            IViewAssetReferenceProvider assetReferenceProvider,
            IScopeLifetimeProvider scopeLifetimeProvider)
        {
            _assetLoader = assetLoader;
            _assetReferenceProvider = assetReferenceProvider;
            _scopeLifetimeProvider = scopeLifetimeProvider;
        }
        
        public async UniTask<T> GetViewAsync<T>() where T : MonoBehaviour, IView
        {
            var viewReference = _assetReferenceProvider.GetAssetReference(typeof(T));

            var view = await _assetLoader.LoadWithComponentAsync<T>(viewReference, _scopeLifetimeProvider.ScopeLifetime);
            
            return view;
        }

        public async UniTask<IView> GetViewAsync(Type viewType)
        {
            var viewReference = _assetReferenceProvider.GetAssetReference(viewType);
            
            var view = await _assetLoader.LoadAsync<Object>(viewReference, _scopeLifetimeProvider.ScopeLifetime);
            
            return (IView) view;
        }
    }
}