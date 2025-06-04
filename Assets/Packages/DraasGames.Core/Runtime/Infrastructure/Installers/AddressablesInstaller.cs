#if DRAASGAMES_ADDRESSABLES_MODULE
using DraasGames.Core.Runtime.Infrastructure.Extensions;
using DraasGames.Core.Runtime.Infrastructure.Loaders.Concrete;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace DraasGames.Core.Runtime.Infrastructure.Installers
{
    public class AddressablesInstaller : MonoInstaller
    {
        [SerializeField]
        [InfoBox("Installer should not be placed on ProjectContext if MoveIntoAllSubcontainers is disabled" +
                 " or placed at scene context if MoveIntoAllSubcontainers is enabled!",
            InfoMessageType.Error,
            VisibleIf = nameof(ShouldShowProjectContextError))]
        private bool _moveIntoAllSubcontainers = false;

        public override void InstallBindings()
        {
            Container
                .BindInterfacesTo<AddressablesAssetLoader>()
                .AsSingle();

            Container
                .BindInterfacesTo<ScopeLifetimeProvider>()
                .AsSingle()
                .MoveIntoAllSubContainersConditional(_moveIntoAllSubcontainers);
        }

        private bool ShouldShowProjectContextError()
        {
            return !IsProjectContext() && _moveIntoAllSubcontainers;
        }

        private bool IsProjectContext()
        {
            return gameObject.name == "ProjectContext" || transform.root.name == "ProjectContext";
        }
    }
}
#endif