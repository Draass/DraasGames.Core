using System;
using Cysharp.Threading.Tasks;
using DraasGames.Core.Runtime.UI.Views.Abstract;
using DraasGames.Core.Runtime.UI.Views.Concrete.ViewContainers;
using UnityEngine;
using Zenject;

namespace DraasGames.Core.Runtime.UI.Views.Concrete.ViewProviders
{
    public class ResourcesViewProviderAsync : IViewProviderAsync
    {
        private readonly ResourcesViewContainer _resourcesViewContainer;
        
        [Inject]
        public ResourcesViewProviderAsync(ResourcesViewContainer resourcesViewContainer)
        {
            _resourcesViewContainer = resourcesViewContainer;
        }
        
        public async UniTask<T> GetViewAsync<T>() where T : MonoBehaviour, IView
        {
            var r = await Resources.LoadAsync<GameObject>(_resourcesViewContainer.GetViewPath<T>()) as GameObject;
            
            return r.GetComponent<T>();
        }

        public async UniTask<IView> GetViewAsync(Type viewType)
        {
            var r = await Resources.LoadAsync<GameObject>(_resourcesViewContainer.GetViewPath(viewType)) as MonoBehaviour;
            
            return r as IView;
        }
    }
}