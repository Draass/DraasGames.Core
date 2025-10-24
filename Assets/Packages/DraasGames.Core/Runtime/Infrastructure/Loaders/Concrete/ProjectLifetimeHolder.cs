using DraasGames.Core.Runtime.Infrastructure.Loaders.Abstract;

namespace DraasGames.Core.Runtime.Infrastructure.Loaders.Concrete
{
    public static class ProjectLifetimeHolder
    {
        public static ILifetime ProjectLifeTime { get; } = new Lifetime();
    }
}