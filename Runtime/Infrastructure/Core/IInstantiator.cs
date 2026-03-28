using UnityEngine;

namespace DraasGames.Core.Runtime.Infrastructure.Core
{
    public interface IInstantiator
    {
        T Instantiate<T>();

        T InstantiatePrefabForComponent<T>(Component prefab);
    }
}
