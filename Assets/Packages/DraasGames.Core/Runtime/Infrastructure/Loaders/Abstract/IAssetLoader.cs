using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace DraasGames.Core.Runtime.Infrastructure.Loaders.Abstract
{
    public interface IAssetLoader<T>
    {
        public UniTask<T> LoadAssetAsync(string path);
    }

    public interface IAssetLoader
    {
        public UniTask<T> LoadAsync<T>(AssetReference assetReference, ILifetime lifetime);

        public UniTask<T> LoadAsync<T>(string key, ILifetime lifetime);

        public UniTask<T> LoadWithComponentAsync<T>(AssetReference assetReference, ILifetime lifetime)
            where T : Component;

        public UniTask<T> LoadWithComponentAsync<T>(string key, ILifetime lifetime) where T : Component;
    }
}