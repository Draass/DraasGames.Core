namespace DraasGames.Core.Runtime.Infrastructure.Core
{
    /// <summary>
    /// Interface for resetting object to it's initial state
    /// </summary>
    public interface IResettable
    {
        // Reset object state to default values
        public void Reset();
    }
}