using DraasGames.Core.Runtime.UI.PresenterNavigationService.Concrete;
using DraasGames.Core.Runtime.UI.Views.Concrete;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace DraasGames.Core.Runtime.Infrastructure.Installers
{
    public class AddressablesViewInstaller : MonoInstaller
    {
        [SerializeField] private AddressablesViewContainer _viewContainer;

        public override void InstallBindings()
        {
            Container
                .BindInterfacesAndSelfTo<ViewFactory>()
                .AsSingle()
                .MoveIntoAllSubContainers();

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
                .BindInterfacesTo<AddressablesViewProvider>()
                .AsSingle()
                .MoveIntoAllSubContainers();

            Container
                .BindInterfacesTo<AddressablesViewContainer>()
                .FromInstance(_viewContainer)
                .AsSingle()
                .MoveIntoAllSubContainers();
        }
    }
}