using System;
using DraasGames.Core.Runtime.Infrastructure.Core;
using Reflex.Core;
using Reflex.Injectors;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DraasGames.Core.Runtime.Infrastructure.Reflex
{
    public class ReflexInstantiatorAdapter : IInstantiator
    {
        private readonly Container _container;

        public ReflexInstantiatorAdapter(Container container)
        {
            _container = container;
        }

        public T Instantiate<T>()
        {
            return (T)ConstructorInjector.Construct(typeof(T), _container);
        }

        public T InstantiatePrefabForComponent<T>(Component prefab)
        {
            if (prefab == null)
            {
                throw new ArgumentNullException(nameof(prefab));
            }

            var instance = Object.Instantiate(prefab);
            GameObjectInjector.InjectRecursive(instance.gameObject, _container);

            if (instance is T contract)
            {
                return contract;
            }

            foreach (var behaviour in instance.gameObject.GetComponentsInChildren<MonoBehaviour>(true))
            {
                if (behaviour is T resolved)
                {
                    return resolved;
                }
            }

            throw new InvalidOperationException(
                $"Instantiated prefab {instance.name} does not contain a component implementing {typeof(T).Name}.");
        }
    }
}
