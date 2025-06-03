using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace DraasGames.Core.Runtime.Infrastructure.Loaders.Abstract
{
    public interface IAssetLoader
    {
        /// <summary>
        /// Load asset by AssetReference
        /// </summary>
        /// <param name="assetReference"></param>
        /// <param name="lifetime">Asset lifetime. If lifetime is null, it will use ProjectLifeTime and will never be unloaded</param>
        /// <param name="onLoadProgress"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public UniTask<T> LoadAsync<T>(AssetReference assetReference, ILifetime lifetime = null, Action<float> onLoadProgress = null);

        /// <summary>
        /// Load asset by key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="lifetime">Asset lifetime. If lifetime is null, it will use ProjectLifeTime and will never be unloaded</param>
        /// <param name="onLoadProgress"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public UniTask<T> LoadAsync<T>(string key, ILifetime lifetime = null, Action<float> onLoadProgress = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assetReference"></param>
        /// <param name="lifetime">Asset lifetime. If lifetime is null, it will use ProjectLifeTime and will never be unloaded</param>
        /// <param name="onLoadProgress"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public UniTask<T> LoadWithComponentAsync<T>(AssetReference assetReference, ILifetime lifetime = null, Action<float> onLoadProgress = null)
            where T : Component;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="lifetime">Asset lifetime. If lifetime is null, it will use ProjectLifeTime and will never be unloaded</param>
        /// <param name="onLoadProgress"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public UniTask<T> LoadWithComponentAsync<T>(string key, ILifetime lifetime = null, Action<float> onLoadProgress = null) 
            where T : Component;
    }
}