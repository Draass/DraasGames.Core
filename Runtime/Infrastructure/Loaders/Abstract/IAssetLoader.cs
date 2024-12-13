using Cysharp.Threading.Tasks;

namespace DraasGames.Core.Runtime.Infrastructure.Loaders.Abstract
{
    public interface IAssetLoader<T>
    {
        public UniTask<T> LoadAssetAsync(string path);
    }
}