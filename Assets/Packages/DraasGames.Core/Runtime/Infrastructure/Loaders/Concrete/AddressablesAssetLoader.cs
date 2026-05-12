#if DRAASGAMES_ADDRESSABLES_ENABLED
using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DraasGames.Core.Runtime.Infrastructure.Loaders.Abstract;
using DraasGames.Core.Runtime.Infrastructure.Logger;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace DraasGames.Core.Runtime.Infrastructure.Loaders.Concrete
{
    public class AddressablesAssetLoader : IAssetLoader, IDisposable
    {
        private readonly Dictionary<ILifetime, List<AsyncOperationHandle>> _handles = new();

        public UniTask<T> LoadAsync<T>(AssetReference assetReference, ILifetime lifetime = null,
            Action<float> onProgress = null)
        {
            if (assetReference == null)
            {
                DLogger.LogError($"Asset reference is null for type: {typeof(T)}!", this);
                return UniTask.FromResult(default(T));
            }

            AsyncOperationHandle<T> handle;
            
            try
            {
                handle = Addressables.LoadAssetAsync<T>(assetReference);
            }
            catch (Exception e)
            {
                DLogger.LogError($"Failed to load asset reference! Exception: {e}", this);
                return UniTask.FromResult(default(T));
            }
            
            return TrackAwaitWithProgress(lifetime, handle, onProgress);
        }

        public UniTask<T> LoadAsync<T>(string key, ILifetime lifetime = null, Action<float> onProgress = null)
        {
            AsyncOperationHandle<T> handle;
            
            try
            {
                handle = Addressables.LoadAssetAsync<T>(key);
            }
            catch (Exception e)
            {
                DLogger.LogError($"Failed to load asset reference! Exception: {e}", this);
                return UniTask.FromResult(default(T));
            }
            
            return TrackAwaitWithProgress(lifetime, handle, onProgress);
        }

        public async UniTask<T> LoadWithComponentAsync<T>(AssetReference assetReference, ILifetime lifetime = null,
            Action<float> onProgress = null) where T : Component
        {
            if (assetReference == null)
            {
                DLogger.LogError($"Asset reference is null for type: {typeof(T)}!", this);
                return null;
            }
            
            var go = await LoadAsync<GameObject>(assetReference, lifetime, onProgress);
            
            if (!go.TryGetComponent<T>(out var component))
            {
                throw new NullReferenceException($"Can't get {typeof(T).Name} from {go.name}");
            }
            
            return component;
        }

        public async UniTask<T> LoadWithComponentAsync<T>(string key, ILifetime lifetime = null,
            Action<float> onProgress = null) where T : Component
        {
            var go = await LoadAsync<GameObject>(key, lifetime, onProgress);
            
            if (!go.TryGetComponent<T>(out var component))
            {
                throw new NullReferenceException($"Can't get {typeof(T).Name} from {go.name}");
            }
            
            return component;
        }

        private async UniTask<T> TrackAwaitWithProgress<T>(ILifetime lifetime, AsyncOperationHandle<T> handle,
            Action<float> onProgress)
        {
            lifetime ??= ProjectLifetimeHolder.ProjectLifeTime;
            
            RegisterHandle(lifetime, handle);
            
            if (onProgress != null)
            {
                while (!handle.IsDone && !lifetime.Token.IsCancellationRequested)
                {
                    onProgress(handle.PercentComplete);
                    await UniTask.Yield(PlayerLoopTiming.Update, lifetime.Token);
                }

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    onProgress(1f);
                }
            }
            
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
                    var (self, lt) = ((AddressablesAssetLoader, ILifetime)) state!;
                    self.ReleaseHandles(lt);
                }, (this, lifetime));
            }

            list.Add(handle);
        }

        private void ReleaseHandles(ILifetime lifetime)
        {
            if (!_handles.TryGetValue(lifetime, out var list))
            {
                return;
            }

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
            var lifetimes = _handles.Keys.ToArray();  
            
            foreach (var lt in lifetimes)
            {
                lt.Dispose();
            }
            
            _handles.Clear();
        }
    }
}
#endif