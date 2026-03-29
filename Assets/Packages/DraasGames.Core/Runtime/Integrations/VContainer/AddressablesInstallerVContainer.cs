#if DRAASGAMES_ADDRESSABLES_ENABLED
using DraasGames.Core.Runtime.Infrastructure.VContainer;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using VContainerLifetime = VContainer.Lifetime;

namespace DraasGames.Core.Runtime.Infrastructure.Installers
{
    public class AddressablesInstallerVContainer : LifetimeScope
    {
        [SerializeField]
        private VContainerLifetime _lifetime = VContainerLifetime.Scoped;

        protected override void Configure(IContainerBuilder builder)
        {
            VContainerInstallerRegistrations.InstallAddressables(builder, _lifetime);
        }
    }
}
#endif
