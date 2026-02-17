using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DraasGames.Core.Runtime.Infrastructure.Loaders.Abstract;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;
using Zenject;
using Object = UnityEngine.Object;

namespace DraasGames.Core.Runtime.Pooling
{
    public class PoolService : IPoolService
    {
        private readonly Dictionary<Type, object> _pools = new();

        private readonly IInstantiator _instantiator;
        private readonly IAssetLoader _resourceLoadService;
        private readonly IPoolTransformProvider _transformProvider;

        public PoolService(
            IInstantiator instantiator, 
            IAssetLoader resourceLoadService, 
            IPoolTransformProvider transformProvider)
        {
            _instantiator = instantiator;
            _resourceLoadService = resourceLoadService; 
            _transformProvider = transformProvider;
        }

        public void RegisterPool<T>(T prefab, PoolOptions<T> options = null) where T : Component, IPoolable
        {
            if (prefab == null)
            {
                Debug.LogError($"Prefab for {typeof(T)} is null!");
                return;
            }

            if (ExistsPool<T>())
            {
                Debug.LogWarning($"Pool for {typeof(T)} already exists!");
                return;
            }
            
            CreatePoolForOptions(prefab, options);
        }
        public async UniTask RegisterPoolAsync<T>(AssetReferenceT<T> assetReference, PoolOptions<T> options = null) 
            where T : Component, IPoolable
        {
            if (assetReference == null)
            {
                Debug.LogError($"Asset reference for {typeof(T)} is null!");
                return;
            }

            if (ExistsPool<T>())
            {
                Debug.LogWarning($"Pool for {typeof(T)} already exists!");
                return;
            }

            var prefab = await _resourceLoadService.LoadAsync<T>(assetReference);
            
            RegisterPool(prefab, options);
        }

        public IObjectPool<T> GetPool<T>() where T : Component, IPoolable
        {
            if (_pools.TryGetValue(typeof(T), out var pool))
            {
                if (pool is IObjectPool<T> poolT)
                {
                    return poolT;
                }
                else
                {
                    Debug.LogError($"Pool for {typeof(T)} is not of type {typeof(IObjectPool<T>)}!");
                    return null;
                }
            }

            Debug.LogError($"Pool for {typeof(T)} not found!");
            return null;
        }

        public bool ExistsPool<T>() where T : Component, IPoolable
        {
            return _pools.ContainsKey(typeof(T));
        }

        public void RemovePool<T>() where T : Component, IPoolable
        {
            if (ExistsPool<T>())
            {
                Debug.LogWarning($"Trying to remove pool for type {typeof(T)} that doesn't exist!");
                return;
            }
            
            var type = typeof(T);
            var pool = _pools[type] as IObjectPool<T>;
            pool.Clear();
            _pools.Remove(type);
        }
        
        private void CreatePoolForOptions<T>(T prefab, PoolOptions<T> options) where T : Component, IPoolable
        {
            IObjectPool<T> pool = null;
            
            if (options == null)
            {
                pool = new ObjectPool<T>(() => OnCreate(prefab), OnGet, OnRelease, OnDestroy);
            }
            else
            {
                var createFunc = options.CreateFunc ?? CreateFunc<T>(prefab);
                var actionOnGet = options.ActionOnGet ?? OnGet<T>;
                var actionOnRelease = options.ActionOnRelease ?? OnRelease<T>;
                var actionOnDestroy = options.ActionOnDestroy ?? OnDestroy<T>;
                
                pool = CreatePool(options.PoolType,createFunc, actionOnGet, actionOnRelease, actionOnDestroy, options.CollectionCheck, options.DefaultCapacity, options.MaxSize);
            }
            
            _pools.Add(typeof(T), pool);
        }
        
        private IObjectPool<T> CreatePool<T>(
            PoolType poolType, 
            Func<T> createFunc, 
            Action<T> actionOnGet, 
            Action<T> actionOnRelease, 
            Action<T> actionOnDestroy,
            bool collectionCheck, 
            int defaultCapacity, 
            int maxSize) where T : Component, IPoolable
        {
            IObjectPool<T> newPool;
                
            if(poolType == PoolType.Stack)
            {
                newPool = new ObjectPool<T>(createFunc, actionOnGet, actionOnRelease, actionOnDestroy, collectionCheck, defaultCapacity, maxSize);
            }
            else
            {
                newPool = new LinkedPool<T>(createFunc, actionOnGet, actionOnRelease, actionOnDestroy, collectionCheck, maxSize);
            }

            return newPool;
        }

        private Func<T> CreateFunc<T>(T prefab) where T : Component, IPoolable
        {
            return () => OnCreate(prefab);
        }

        private T OnCreate<T>(T prefab) where T : Component, IPoolable
        {
            return _instantiator.InstantiatePrefabForComponent<T>(prefab);
        }

        private void OnGet<T>(T obj) where T : Component, IPoolable
        {
            obj.Enable();
        }

        private void OnRelease<T>(T obj) where T : Component, IPoolable
        {
            obj.Clear();
            obj.Disable();
            obj.transform.SetParent(_transformProvider.PoolContainer, false);
        }

        private void OnDestroy<T>(T obj) where T : Component, IPoolable
        {
            Object.Destroy(obj.gameObject);
        }
    }
}