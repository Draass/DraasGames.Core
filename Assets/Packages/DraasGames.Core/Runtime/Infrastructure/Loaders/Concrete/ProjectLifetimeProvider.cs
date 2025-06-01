using System;
using DraasGames.Core.Runtime.Infrastructure.Loaders.Abstract;

namespace DraasGames.Core.Runtime.Infrastructure.Loaders.Concrete
{
    public class ProjectLifetimeProvider : IProjectLifetimeProvider, IDisposable
    {
        public ILifetime Lifetime { get; } = new Lifetime();

        public void Dispose()
        {
            Lifetime?.Dispose();
        }
    }
}