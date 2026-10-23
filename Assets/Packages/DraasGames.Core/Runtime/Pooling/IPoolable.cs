namespace DraasGames.Core.Runtime.Pooling
{
    public interface IPoolable
    {
        void Enable();
        void Disable();
        
        /// <summary>
        /// Clear pooled object before releasing it
        /// </summary>
        void Clear();
    }
}