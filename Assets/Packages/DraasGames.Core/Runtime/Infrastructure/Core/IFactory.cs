namespace DraasGames.Core.Runtime.Infrastructure.Core
{
    public interface IFactory<out T>
    {
        public T Create();
    }
}