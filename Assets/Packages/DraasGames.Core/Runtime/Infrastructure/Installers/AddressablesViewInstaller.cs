#if DRAASGAMES_ADDRESSABLES_ENABLED
using DraasGames.Core.Runtime.Infrastructure.Extensions;
using DraasGames.Core.Runtime.UI.PresenterNavigationService.Concrete;
using DraasGames.Core.Runtime.UI.Views.Concrete;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace DraasGames.Core.Runtime.Infrastructure.Installers
{
    public class AddressablesViewInstaller : MonoInstaller
    {
        [SerializeField, Required, AssetsOnly] 
        private AddressablesViewContainer _viewContainer;

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
                .BindInterfacesTo<AddressablesViewProvider>()
                .AsSingle()
                .MoveIntoAllSubContainersConditional(_moveIntoAllSubContainers);

            Container
                .BindInterfacesTo<AddressablesViewContainer>()
                .FromInstance(_viewContainer)
                .AsSingle()
                .MoveIntoAllSubContainersConditional(_moveIntoAllSubContainers);
        }
        
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
#endif