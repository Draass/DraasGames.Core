using System;
using DraasGames.Core.Runtime.Infrastructure.Loaders.Abstract;

namespace DraasGames.Core.Runtime.Infrastructure.Loaders.Concrete
{
    public class SceneLifetimeProvider : ISceneLifetimeProvider, IDisposable
    {
        public ILifetime Lifetime { get; } = new Lifetime();

        public void Dispose()
        {
            Lifetime?.Dispose();
        }
    }
}