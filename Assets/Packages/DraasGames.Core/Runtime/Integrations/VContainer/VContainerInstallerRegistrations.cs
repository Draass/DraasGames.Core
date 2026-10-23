using DraasGames.Core.Runtime.Infrastructure.Core;
using DraasGames.Core.Runtime.Infrastructure.Installers;
using DraasGames.Core.Runtime.Infrastructure.Loaders.Abstract;
using DraasGames.Core.Runtime.Infrastructure.Loaders.Concrete;
using DraasGames.Core.Runtime.UI.PresenterNavigationService.Abstract;
using DraasGames.Core.Runtime.UI.PresenterNavigationService.Concrete;
using DraasGames.Core.Runtime.UI.Views.Abstract;
using DraasGames.Core.Runtime.UI.Views.Concrete;
using DraasGames.Core.Runtime.UI.Views.Concrete.ViewContainers;
using DraasGames.Core.Runtime.UI.Views.Concrete.ViewProviders;
using VContainer;
using VContainerLifetime = VContainer.Lifetime;

namespace DraasGames.Core.Runtime.Infrastructure.VContainer
{
    internal static class VContainerInstallerRegistrations
    {
        public static void InstallResourcesViews(
            IContainerBuilder builder,
            ResourcesViewContainer resourcesViewContainer,
            VContainerLifetime lifetime = VContainerLifetime.Singleton)
        {
            builder.Register<IInstantiator>(resolver => new VContainerInstantiatorAdapter(resolver), lifetime);
            builder.Register<IViewFactory>(resolver => new ViewFactory(
                resolver.Resolve<IInstantiator>(),
                resolver.Resolve<IViewProvider>()), lifetime);
            builder.Register<IViewRouter>(resolver => new ViewRouter(
                resolver.Resolve<IViewFactory>()), lifetime);
            builder.Register<IPresenterNavigationService>(resolver => new PresenterNavigationService(
                resolver.Resolve<IInstantiator>()), lifetime);
            builder.RegisterInstance(resourcesViewContainer);
            builder.Register<IViewProvider>(resolver => new ResourcesViewProviderAsync(
                resolver.Resolve<ResourcesViewContainer>()), lifetime);
        }

#if DRAASGAMES_ADDRESSABLES_ENABLED
        public static void InstallAddressables(
            IContainerBuilder builder,
            VContainerLifetime lifetime = VContainerLifetime.Singleton)
        {
            builder.Register<IAssetLoader, AddressablesAssetLoader>(lifetime);
            builder.Register<IScopeLifetimeProvider, ScopeLifetimeProvider>(lifetime);
        }

        public static void InstallAddressablesViews(
            IContainerBuilder builder,
            AddressablesViewContainer viewContainer,
            VContainerLifetime lifetime = VContainerLifetime.Singleton)
        {
            builder.Register<IInstantiator>(resolver => new VContainerInstantiatorAdapter(resolver), lifetime);
            builder.Register<IViewFactory>(resolver => new ViewFactory(
                resolver.Resolve<IInstantiator>(),
                resolver.Resolve<IViewProvider>()), lifetime);
            builder.Register<IViewRouter>(resolver => new ViewRouter(
                resolver.Resolve<IViewFactory>()), lifetime);
            builder.Register<IPresenterNavigationService>(resolver => new PresenterNavigationService(
                resolver.Resolve<IInstantiator>()), lifetime);
            builder.RegisterInstance(viewContainer);
            builder.Register<IViewProvider>(resolver => new AddressablesViewProvider(
                resolver.Resolve<IAssetLoader>(),
                resolver.Resolve<AddressablesViewContainer>(),
                resolver.Resolve<IScopeLifetimeProvider>()), lifetime);
        }
#endif
    }
}
