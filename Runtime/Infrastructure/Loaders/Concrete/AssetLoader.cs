using Cysharp.Threading.Tasks;
using DraasGames.Core.Runtime.Infrastructure.Loaders.Abstract;
using UnityEngine;

namespace DraasGames.Core.Runtime.Infrastructure.Loaders.Concrete
{
    public class AssetLoader : IPrefabLoader
    {
        public async UniTask<GameObject> LoadAssetAsync(string path)
        {
            var asset = await Resources.LoadAsync<GameObject>(path);

            return (GameObject) asset;
        }
    }
}