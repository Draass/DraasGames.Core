using System;
using UnityEngine.AddressableAssets;

namespace DraasGames.Core.Runtime.UI.Views.Abstract
{
    public interface IViewAssetReferenceProvider
    {
        public AssetReference GetAssetReference(Type type);
    }
}