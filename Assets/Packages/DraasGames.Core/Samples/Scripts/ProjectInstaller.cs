using DraasGames.Core.Runtime.Infrastructure.Loaders.Abstract;
using Zenject;

namespace DraasGames.Core.Samples.Scripts
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<AddressablesAssetLoader>().AsSingle();
        }
    }
}