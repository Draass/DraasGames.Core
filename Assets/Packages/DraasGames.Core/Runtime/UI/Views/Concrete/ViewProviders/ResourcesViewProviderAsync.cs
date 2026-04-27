using System;
using Cysharp.Threading.Tasks;
using DraasGames.Core.Runtime.UI.Views.Abstract;
using DraasGames.Core.Runtime.UI.Views.Concrete.ViewContainers;
using UnityEngine;

namespace DraasGames.Core.Runtime.UI.Views.Concrete.ViewProviders
{
    public class ResourcesViewProviderAsync : IViewProvider
    {
        private readonly ResourcesViewContainer _resourcesViewContainer;

        public ResourcesViewProviderAsync(ResourcesViewContainer resourcesViewContainer)
        {
            _resourcesViewContainer = resourcesViewContainer;
        }

        public async UniTask<T> GetViewAsync<T>() where T : IViewBase
        {
            var viewPath = _resourcesViewContainer.GetViewPath<T>();

            var r = await Resources.LoadAsync<GameObject>(viewPath) as GameObject;

            var component = r.GetComponent(typeof(T)) as IViewBase;
            if (component is not T typedView)
            {
                throw new NullReferenceException($"Can't get {typeof(T).Name} from {r.name}");
            }

            return typedView;
        }

        public async UniTask<IViewBase> GetViewAsync(Type viewType)
        {
            var viewPath = _resourcesViewContainer.GetViewPath(viewType);

            var r = await Resources.LoadAsync<GameObject>(viewPath) as GameObject;

            var component = r.GetComponent(viewType) as IViewBase;
            if (component == null)
            {
                throw new NullReferenceException($"Can't get {viewType.Name} from {r.name}");
            }

            return component;
        }
    }
}
