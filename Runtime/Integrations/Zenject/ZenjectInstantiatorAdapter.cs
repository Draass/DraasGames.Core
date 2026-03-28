using UnityEngine;
using ZenjectInstantiator = Zenject.IInstantiator;

namespace DraasGames.Core.Runtime.Infrastructure.Core
{
    public class ZenjectInstantiatorAdapter : IInstantiator
    {
        private readonly ZenjectInstantiator _instantiator;

        public ZenjectInstantiatorAdapter(ZenjectInstantiator instantiator)
        {
            _instantiator = instantiator;
        }

        public T Instantiate<T>()
        {
            return _instantiator.Instantiate<T>();
        }

        public T InstantiatePrefabForComponent<T>(Component prefab)
        {
            return _instantiator.InstantiatePrefabForComponent<T>(prefab);
        }
    }
}
