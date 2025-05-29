using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DraasGames.Core.Runtime.Infrastructure.Loaders.Abstract;
using DraasGames.Core.Runtime.Infrastructure.Loaders.Concrete;
using DraasGames.Core.Runtime.UI.Views.Abstract;
using DraasGames.Core.Runtime.UI.Views.Concrete;
using DraasGames.Core.Runtime.UI.Views.Concrete.ViewContainers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DraasGames.Core.Runtime.Infrastructure.Installers
{
    public class AddressablesViewProvider : IViewProvider
    {
        private readonly IAssetLoader _assetLoader;
        private readonly IViewAssetReferenceProvider _assetReferenceProvider;

        public AddressablesViewProvider(
            IAssetLoader assetLoader, 
            IViewAssetReferenceProvider assetReferenceProvider)
        {
            _assetLoader = assetLoader;
            _assetReferenceProvider = assetReferenceProvider;
        }
        
        public async UniTask<T> GetViewAsync<T>() where T : MonoBehaviour, IView
        {
            var viewReference = _assetReferenceProvider.GetAssetReference(typeof(T));

            var view = await _assetLoader.LoadWithComponentAsync<T>(viewReference, new Lifetime());
            
            return view;
        }

        public async UniTask<IView> GetViewAsync(Type viewType)
        {
            var viewReference = _assetReferenceProvider.GetAssetReference(viewType);
            
            var view = await _assetLoader.LoadAsync<Object>(viewReference, new Lifetime());
            
            return (IView) view;
        }
    }
}