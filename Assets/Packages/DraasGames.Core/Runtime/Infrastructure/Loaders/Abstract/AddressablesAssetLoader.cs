#if DRAASGAMES_ADDRESSABLES_MODULE
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DraasGames.Core.Runtime.Infrastructure.Logger;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace DraasGames.Core.Runtime.Infrastructure.Loaders.Abstract
{
    public class AddressablesAssetLoader : IAssetLoader
    {
        private readonly Dictionary<ILifetime, List<AsyncOperationHandle>> _handles = new();

        public async UniTask<T> LoadAsync<T>(AssetReference assetReference, ILifetime lifetime)
        {
            var handle = Addressables.LoadAssetAsync<T>(assetReference);
            return await TrackAndAwait(lifetime, handle);
        }

        public async UniTask<T> LoadAsync<T>(string key, ILifetime lifetime)
        {
            var handle = Addressables.LoadAssetAsync<T>(key);
            return await TrackAndAwait(lifetime, handle);
        }

        public async UniTask<T> LoadWithComponentAsync<T>(AssetReference assetReference, ILifetime lifetime)
            where T : Component
        {
            var go = await LoadAsync<GameObject>(assetReference, lifetime);
            
            if (!go.TryGetComponent<T>(out var component))
            {
                throw new NullReferenceException($"Can't get {typeof(T).Name} from {go.name}");
            }
            
            return component;
        }

        public async UniTask<T> LoadWithComponentAsync<T>(string key, ILifetime lifetime)
            where T : Component
        {
            var go = await LoadAsync<GameObject>(key, lifetime);
            
            if (!go.TryGetComponent<T>(out var component))
            {
                throw new NullReferenceException($"Can't get {typeof(T).Name} from {go.name}");
            }
            
            return component;
        }

        private async UniTask<T> Load<T>(ILifetime lifetime, AsyncOperationHandle<T> handle)
        {
            StoreHandleAsync(lifetime, handle);
            return await handle.Task;
        }

        private void StoreHandleAsync<T>(ILifetime lifetime, AsyncOperationHandle<T> handle)
        {
            if (lifetime == null)
            {
                DLogger.LogError("Lifetime is null");
                
                return;
            }
            
            if (!_handles.ContainsKey(lifetime))
            {
                RemoveHandlesOnLifetimeEnd(lifetime).Forget();
            }
            
            if(_handles.TryGetValue(lifetime, out var handles))
            {
                handles.Add(handle);
            }
            else
            {
                _handles.Add(lifetime, new List<AsyncOperationHandle>{handle});
            }
        }

        private async UniTaskVoid RemoveHandlesOnLifetimeEnd(ILifetime lifetime)
        {
            await lifetime.WaitForEnd();
            RemoveHandlesForLifetime(lifetime);
        }

        private void RemoveHandlesForLifetime(ILifetime lifetime)
        {
            foreach (var handle in _handles[lifetime])
            {
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                }
            }

            _handles.Remove(lifetime);
        }
        
        private async UniTask<T> TrackAndAwait<T>(ILifetime lifetime, AsyncOperationHandle<T> handle)
        {
            RegisterHandle(lifetime, handle);
            return await handle.Task;
        }

        private void RegisterHandle<T>(ILifetime lifetime, AsyncOperationHandle<T> handle)
        {
            if (lifetime == null)
            {
                throw new ArgumentNullException(nameof(lifetime));
            }

            if (!_handles.TryGetValue(lifetime, out var list))
            {
                list = ListPool<AsyncOperationHandle>.Get();
                _handles.Add(lifetime, list);

                lifetime.Token.Register(static state =>
                {
                    var (self, lt) = ((AddressablesAssetLoader, ILifetime))state!;
                    self.ReleaseHandles(lt);
                }, (this, lifetime));
            }

            list.Add(handle);
        }

        private void ReleaseHandles(ILifetime lifetime)
        {
            if (!_handles.TryGetValue(lifetime, out var list)) return;

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].IsValid())
                {
                    Addressables.Release(list[i]);
                }
            }

            list.Clear();
            ListPool<AsyncOperationHandle>.Release(list);
            _handles.Remove(lifetime);
        }

        public void Dispose()
        {
            foreach (var kvp in _handles)
            {
                kvp.Key.Dispose();
            }
            
            _handles.Clear();
        }
    }
}
#endif