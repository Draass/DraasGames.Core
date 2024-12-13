#if DRAASGAMES_ADDRESSABLES_MODULE
using DraasGames.Runtime.Addressables;
#endif
using DraasGames.Core.Runtime.UI.Views.Concrete;
using DraasGames.Core.Runtime.UI.Views.Concrete.ViewContainers;
using DraasGames.Core.Runtime.UI.Views.Concrete.ViewProviders;
using UnityEngine;
using Zenject;

namespace DraasGames.Core.Runtime.Infrastructure.Installers
{
    public class ViewInstaller : MonoInstaller
    {
#if DRAASGAMES_ADDRESSABLES_MODULE
        private enum ViewRetrievalMode
        {
            Addressables,
            Resources
        }

        [SerializeField, BoxGroup("Settings")] 
        private ViewRetrievalMode _viewRetrievalMode = ViewRetrievalMode.Addressables;
        
        [SerializeField] private AddressablesViewContainer _viewContainer;
#endif

        [SerializeField] private ResourcesViewContainer _resourcesViewContainer;
        
        public override void InstallBindings()
        {
            Container
                .BindInterfacesAndSelfTo<ViewFactory>()
                .FromNew()
                .AsSingle();
            
            Container
                .BindInterfacesAndSelfTo<ViewRouter>()
                .FromNew()
                .AsSingle();
            
            BindViewRetrieval();
        }

        private void BindViewRetrieval()
        {
#if DRAASGAMES_ADDRESSABLES_MODULE
            if (_viewRetrievalMode == ViewRetrievalMode.Addressables)
            {
                Container
                    .Bind<AddressablesViewContainer>()
                    .FromInstance(_viewContainer)
                    .AsSingle();

                Container
                    .BindInterfacesTo<AddressablesViewProviderAsync>()
                    .FromNew()
                    .AsSingle();
            }
            else if(_viewRetrievalMode == ViewRetrievalMode.Resources)
            {
                Container
                    .Bind<ResourcesViewContainer>()
                    .FromInstance(_resourcesViewContainer)
                    .AsSingle();
                
                Container
                    .BindInterfacesTo<ResourcesViewProviderAsync>()
                    .FromNew()
                    .AsSingle();
            }
            
            
            Container
                .Bind<AddressablesManager>()
                .FromNewComponentOnNewGameObject()
                .AsSingle();
#else 
            Container
                .Bind<ResourcesViewContainer>()
                .FromInstance(_resourcesViewContainer)
                .AsSingle();
                
            Container
                .BindInterfacesTo<ResourcesViewProviderAsync>()
                .FromNew()
                .AsSingle();
#endif
        }
    }
}