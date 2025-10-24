using DraasGames.Core.Runtime.Infrastructure.Extensions;
using DraasGames.Core.Runtime.UI.PresenterNavigationService.Concrete;
using DraasGames.Core.Runtime.UI.Views.Concrete;
using DraasGames.Core.Runtime.UI.Views.Concrete.ViewContainers;
using DraasGames.Core.Runtime.UI.Views.Concrete.ViewProviders;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace DraasGames.Core.Runtime.Infrastructure.Installers
{
    public class ResourcesViewInstaller : MonoInstaller
    {
        [SerializeField, Required, AssetsOnly] 
        private ResourcesViewContainer _resourcesViewContainer;

        [SerializeField]
        [InfoBox("Installer should not be placed on ProjectContext if MoveIntoAllSubcontainers is disabled" +
                 " or placed at scene context if MoveIntoAllSubcontainers is enabled!",
            InfoMessageType.Error,
            VisibleIf = nameof(ShouldShowProjectContextError))]
        private bool _moveIntoAllSubContainers = false;
        
        public override void InstallBindings()
        {
            Container
                .BindInterfacesAndSelfTo<ViewFactory>()
                .FromNew()
                .AsSingle()
                .MoveIntoAllSubContainersConditional(_moveIntoAllSubContainers);
            
            Container
                .BindInterfacesAndSelfTo<ViewRouter>()
                .AsSingle()
                .MoveIntoAllSubContainersConditional(_moveIntoAllSubContainers);

            Container
                .BindInterfacesTo<PresenterNavigationService>()
                .AsSingle()
                .MoveIntoAllSubContainersConditional(_moveIntoAllSubContainers);
            
            BindViewRetrieval();
        }

        private void BindViewRetrieval()
        {
            Container
                .Bind<ResourcesViewContainer>()
                .FromInstance(_resourcesViewContainer)
                .AsSingle()
                .MoveIntoAllSubContainersConditional(_moveIntoAllSubContainers);
                
            Container
                .BindInterfacesTo<ResourcesViewProviderAsync>()
                .FromNew()
                .AsSingle()
                .MoveIntoAllSubContainersConditional(_moveIntoAllSubContainers);
        }
        
        // TODO to general validation method
        private bool ShouldShowProjectContextError()
        {
            return !IsProjectContext() && _moveIntoAllSubContainers;
        }

        private bool IsProjectContext()
        {
            return gameObject.name == "ProjectContext" || transform.root.name == "ProjectContext";
        }
    }
}