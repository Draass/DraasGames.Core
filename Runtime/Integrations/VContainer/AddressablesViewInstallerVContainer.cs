#if DRAASGAMES_ADDRESSABLES_ENABLED
using DraasGames.Core.Runtime.Infrastructure.VContainer;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using VContainerLifetime = VContainer.Lifetime;

namespace DraasGames.Core.Runtime.Infrastructure.Installers
{
    public class AddressablesViewInstallerVContainer : LifetimeScope
    {
        [SerializeField, Required, AssetsOnly]
        private AddressablesViewContainer _viewContainer;

        [SerializeField]
        private VContainerLifetime _lifetime = VContainerLifetime.Scoped;

        protected override void Configure(IContainerBuilder builder)
        {
            VContainerInstallerRegistrations.InstallAddressablesViews(builder, _viewContainer, _lifetime);
        }
    }
}
#endif
