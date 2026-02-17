using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;

namespace DraasGames.Core.Runtime.Pooling
{
    public interface IPoolService
    {
        /// <summary>
        /// Register pool from prefab
        /// </summary>
        /// <param name="prefab">Component prefab</param>
        /// <typeparam name="T">Prefab component type</typeparam>
        void RegisterPool<T>(T prefab, PoolOptions<T> options = null) where T : Component, IPoolable;
        
        /// <summary>
        /// Register pool for asset reference
        /// </summary>
        /// <param name="assetReference">Reference to asset</param>
        /// <typeparam name="T">Prefab component type</typeparam>
        /// <returns></returns>
        UniTask RegisterPoolAsync<T>(AssetReferenceT<T> assetReference, PoolOptions<T> options = null) where T : Component, IPoolable;
        
        /// <summary>
        /// Get pool by type if exitsts. If not, returns null
        /// </summary>
        /// <typeparam name="T">Prefab component type</typeparam>
        /// <returns></returns>
        IObjectPool<T> GetPool<T>() where T : Component, IPoolable;
        
        /// <summary>
        /// Remove pool from registered pools and release all objects from it.
        /// </summary>
        /// <typeparam name="T">Prefab component type</typeparam> и
        void RemovePool<T>() where T : Component, IPoolable;
        
        /// <summary>
        /// Check if pool exists
        /// </summary>
        /// <typeparam name="T">Prefab component type</typeparam>
        /// <returns></returns>
        bool ExistsPool<T>() where T : Component, IPoolable;
    }
}