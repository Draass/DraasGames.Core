namespace DraasGames.Core.Runtime.Infrastructure.Loaders.Abstract
{
    public interface ISceneLifetimeProvider
    {
        public ILifetime Lifetime { get; }
    }
}