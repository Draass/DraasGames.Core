using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace DraasGames.Core.Runtime.Infrastructure.Loaders.Abstract
{
    public interface ILifetime : IDisposable
    {
        CancellationToken Token { get; }
    }
}