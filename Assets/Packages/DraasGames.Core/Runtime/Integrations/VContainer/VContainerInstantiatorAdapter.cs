using System;
using System.Linq;
using System.Reflection;
using DraasGames.Core.Runtime.Infrastructure.Core;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace DraasGames.Core.Runtime.Infrastructure.VContainer
{
    public class VContainerInstantiatorAdapter : IInstantiator
    {
        private readonly IObjectResolver _resolver;
        private static readonly MethodInfo ResolveGenericMethod = typeof(IObjectResolver)
            .GetMethods()
            .First(m => m.Name == "Resolve" && m.IsGenericMethodDefinition && m.GetParameters().Length == 0);

        public VContainerInstantiatorAdapter(IObjectResolver resolver)
        {
            _resolver = resolver;
        }

        public T Instantiate<T>()
        {
            return (T)Construct(typeof(T));
        }

        public T InstantiatePrefabForComponent<T>(Component prefab)
        {
            if (prefab == null)
            {
                throw new ArgumentNullException(nameof(prefab));
            }

            var instance = _resolver.Instantiate(prefab);
            var component = instance.GetComponentInChildren(typeof(T), true);

            if (component is T typedComponent)
            {
                return typedComponent;
            }

            throw new InvalidOperationException(
                $"Instantiated prefab {instance.name} does not contain a component implementing {typeof(T).Name}.");
        }

        private object Construct(Type concreteType)
        {
            var constructors = concreteType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var constructor = constructors
                .OrderByDescending(x => x.GetParameters().Length)
                .FirstOrDefault();

            if (constructor == null)
            {
                throw new InvalidOperationException($"Type {concreteType.Name} does not have a constructor.");
            }

            var parameters = constructor.GetParameters()
                .Select(x => Resolve(x.ParameterType))
                .ToArray();

            return constructor.Invoke(parameters);
        }

        private object Resolve(Type parameterType)
        {
            return ResolveGenericMethod.MakeGenericMethod(parameterType).Invoke(_resolver, null);
        }
    }
}
