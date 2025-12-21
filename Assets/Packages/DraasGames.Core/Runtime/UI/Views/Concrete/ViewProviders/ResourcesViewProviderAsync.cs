using System;
using Cysharp.Threading.Tasks;
using DraasGames.Core.Runtime.UI.Views.Abstract;
using DraasGames.Core.Runtime.UI.Views.Concrete.ViewContainers;
using UnityEngine;
using Zenject;

namespace DraasGames.Core.Runtime.UI.Views.Concrete.ViewProviders
{
    public class ResourcesViewProviderAsync : IViewProvider
    {
        private readonly ResourcesViewContainer _resourcesViewContainer;

        [Inject]
        public ResourcesViewProviderAsync(ResourcesViewContainer resourcesViewContainer)
        {
            _resourcesViewContainer = resourcesViewContainer;
        }

        public async UniTask<T> GetViewAsync<T>() where T : MonoBehaviour, IViewBase
        {
            var viewPath = _resourcesViewContainer.GetViewPath<T>();

            var r = await Resources.LoadAsync<GameObject>(viewPath) as GameObject;

            return r.GetComponent<T>();
        }

        public async UniTask<IViewBase> GetViewAsync(Type viewType)
        {
            var viewPath = _resourcesViewContainer.GetViewPath(viewType);

            var r = await Resources.LoadAsync<GameObject>(viewPath) as MonoBehaviour;

            return r as IViewBase;
        }
    }
}
