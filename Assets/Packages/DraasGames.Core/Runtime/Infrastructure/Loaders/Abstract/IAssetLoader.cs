using System;
using System.Threading;
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

public class Lifetime : ILifetime
{
    public bool IsOngoing => !isDisposed;

    public CancellationToken CancellationToken => _cts.Token;

    private bool isDisposed = false;
    private CancellationTokenSource _cts;

    public Lifetime()
    {
        _cts = new();
    }

    public async UniTask WaitForEnd()
        => await UniTask.WaitUntil(() => !IsOngoing, PlayerLoopTiming.PreUpdate);

    public void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        _cts?.Cancel();
        _cts?.Dispose();

        isDisposed = true;
    }
}

public interface ILifetime : IDisposable
{
    bool IsOngoing { get; }
    CancellationToken CancellationToken { get; }
    UniTask WaitForEnd();
}

public struct LifetimeStruct : IDisposable
{
    public bool IsDisposed { get; private set; }

    public void Dispose()
    {
        IsDisposed = true;
    }
}