namespace DraasGames.Core.Runtime.Infrastructure.Loaders.Abstract
{
    public interface IScopeLifetimeProvider
    {
        ILifetime ScopeLifetime { get; }
    }
}