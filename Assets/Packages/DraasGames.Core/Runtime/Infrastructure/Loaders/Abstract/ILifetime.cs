using System;
using System.Threading;
using Cysharp.Threading.Tasks;

public interface ILifetime : IDisposable
{
    CancellationToken Token { get; }
    UniTask WaitForEnd();
}