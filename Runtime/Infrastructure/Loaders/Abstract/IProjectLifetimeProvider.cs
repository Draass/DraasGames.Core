namespace DraasGames.Core.Runtime.Infrastructure.Loaders.Abstract
{
    public interface IProjectLifetimeProvider
    {
        public ILifetime Lifetime { get; }
    }
}