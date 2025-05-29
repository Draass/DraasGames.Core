using DraasGames.Core.Runtime.UI.PresenterNavigationService.Concrete;
using DraasGames.Core.Runtime.UI.Views.Concrete;
using DraasGames.Core.Runtime.UI.Views.Concrete.ViewContainers;
using DraasGames.Core.Runtime.UI.Views.Concrete.ViewProviders;
using UnityEngine;
using Zenject;

namespace DraasGames.Core.Runtime.Infrastructure.Installers
{
    public class ResourcesViewInstaller : MonoInstaller
    {
        [SerializeField] 
        private ResourcesViewContainer _resourcesViewContainer;

        public override void InstallBindings()
        {
            Container
                .BindInterfacesAndSelfTo<ViewFactory>()
                .FromNew()
                .AsSingle()
                .MoveIntoAllSubContainers();;
            
            Container
                .BindInterfacesAndSelfTo<ViewRouter>()
                .AsSingle()
                .MoveIntoAllSubContainers();

            Container
                .BindInterfacesTo<PresenterNavigationService>()
                .AsSingle()
                .MoveIntoAllSubContainers();
            
            BindViewRetrieval();
        }

        private void BindViewRetrieval()
        {
            Container
                .Bind<ResourcesViewContainer>()
                .FromInstance(_resourcesViewContainer)
                .AsSingle().MoveIntoAllSubContainers();
                
            Container
                .BindInterfacesTo<ResourcesViewProviderAsync>()
                .FromNew()
                .AsSingle().MoveIntoAllSubContainers();
        }
    }
}