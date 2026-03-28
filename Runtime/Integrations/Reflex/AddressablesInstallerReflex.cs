#if DRAASGAMES_ADDRESSABLES_ENABLED
using DraasGames.Core.Runtime.Infrastructure.Loaders.Abstract;
using DraasGames.Core.Runtime.Infrastructure.Loaders.Concrete;
using Reflex.Core;
using Reflex.Enums;
using UnityEngine;
using ReflexResolution = Reflex.Enums.Resolution;
using Lifetime = Reflex.Enums.Lifetime;
using Resolution = Reflex.Enums.Resolution;

namespace DraasGames.Core.Runtime.Infrastructure.Installers
{
    public class AddressablesInstallerReflex : MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder builder)
        {
            builder.RegisterType(
                typeof(AddressablesAssetLoader),
                new[] { typeof(IAssetLoader) },
                Lifetime.Singleton,
                ReflexResolution.Lazy);

            builder.RegisterType(
                typeof(ScopeLifetimeProvider),
                new[] { typeof(IScopeLifetimeProvider) },
                Lifetime.Singleton,
                ReflexResolution.Lazy);
        }
    }
}
#endif
