using DraasGames.Core.Runtime.Infrastructure.VContainer;
using DraasGames.Core.Runtime.UI.Views.Concrete.ViewContainers;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using VContainerLifetime = VContainer.Lifetime;

namespace DraasGames.Core.Runtime.Infrastructure.Installers
{
    public class ResourcesViewInstallerVContainer : LifetimeScope
    {
        [SerializeField, Required, AssetsOnly]
        private ResourcesViewContainer _resourcesViewContainer;

        [SerializeField]
        private VContainerLifetime _lifetime = VContainerLifetime.Singleton;

        protected override void Configure(IContainerBuilder builder)
        {
            VContainerInstallerRegistrations.InstallResourcesViews(builder, _resourcesViewContainer, _lifetime);
        }
    }
}
