using System;
using DraasGames.Core.Runtime.Infrastructure.Loaders.Abstract;

namespace DraasGames.Core.Runtime.Infrastructure.Loaders.Concrete
{
    public class ScopeLifetimeProvider : IScopeLifetimeProvider, IDisposable
    {
        public ILifetime ScopeLifetime { get; } = new Lifetime();

        public void Dispose()
        {
            ScopeLifetime?.Dispose();
        }
    }
}