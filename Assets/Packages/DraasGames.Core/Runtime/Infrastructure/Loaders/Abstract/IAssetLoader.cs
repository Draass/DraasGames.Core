using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace DraasGames.Core.Runtime.Infrastructure.Loaders.Abstract
{
    public interface IAssetLoader
    {
        public UniTask<T> LoadAsync<T>(AssetReference assetReference, ILifetime lifetime, Action<float> onLoadProgress = null);

        public UniTask<T> LoadAsync<T>(string key, ILifetime lifetime, Action<float> onLoadProgress = null);

        public UniTask<T> LoadWithComponentAsync<T>(AssetReference assetReference, ILifetime lifetime, Action<float> onLoadProgress = null)
            where T : Component;

        public UniTask<T> LoadWithComponentAsync<T>(string key, ILifetime lifetime, Action<float> onLoadProgress = null) 
            where T : Component;
    }
}