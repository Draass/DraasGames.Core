using System;
using UnityEngine.AddressableAssets;

namespace DraasGames.Core.Runtime.Infrastructure.Installers
{
    public interface IViewAssetReferenceProvider
    {
        public AssetReference GetAssetReference(Type type);
    }
}