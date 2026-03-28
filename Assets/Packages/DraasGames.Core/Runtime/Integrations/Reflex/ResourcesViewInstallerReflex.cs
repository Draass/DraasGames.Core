using DraasGames.Core.Runtime.Infrastructure.Core;
using DraasGames.Core.Runtime.Infrastructure.Reflex;
using DraasGames.Core.Runtime.UI.PresenterNavigationService.Abstract;
using DraasGames.Core.Runtime.UI.PresenterNavigationService.Concrete;
using DraasGames.Core.Runtime.UI.Views.Abstract;
using DraasGames.Core.Runtime.UI.Views.Concrete;
using DraasGames.Core.Runtime.UI.Views.Concrete.ViewContainers;
using DraasGames.Core.Runtime.UI.Views.Concrete.ViewProviders;
using Reflex.Core;
using Reflex.Enums;
using Sirenix.OdinInspector;
using UnityEngine;
using ReflexResolution = Reflex.Enums.Resolution;

namespace DraasGames.Core.Runtime.Infrastructure.Installers
{
    public class ResourcesViewInstallerReflex : MonoBehaviour, IInstaller
    {
        [SerializeField, Required, AssetsOnly]
        private ResourcesViewContainer _resourcesViewContainer;

        public void InstallBindings(ContainerBuilder builder)
        {
            builder.RegisterFactory(
                container => new ReflexInstantiatorAdapter(container),
                typeof(ReflexInstantiatorAdapter),
                new[] { typeof(IInstantiator) },
                Lifetime.Singleton,
                ReflexResolution.Lazy);

            builder.RegisterType(
                typeof(ViewFactory),
                new[] { typeof(ViewFactory), typeof(IViewFactory) },
                Lifetime.Singleton,
                ReflexResolution.Lazy);

            builder.RegisterType(
                typeof(ViewRouter),
                new[] { typeof(ViewRouter), typeof(IViewRouter) },
                Lifetime.Singleton,
                ReflexResolution.Lazy);

            builder.RegisterType(
                typeof(PresenterNavigationService),
                new[] { typeof(IPresenterNavigationService) },
                Lifetime.Singleton,
                ReflexResolution.Lazy);

            builder.RegisterValue(
                _resourcesViewContainer,
                new[] { typeof(ResourcesViewContainer) });

            builder.RegisterType(
                typeof(ResourcesViewProviderAsync),
                new[] { typeof(IViewProvider) },
                Lifetime.Singleton,
                ReflexResolution.Lazy);
        }
    }
}
