#if UNITY_EDITOR
using System;
using DraasGames.Core.Runtime.UI.Views.Abstract;
using UnityEditor;
using UnityEngine;

namespace DraasGames.Core.Runtime.UI.Views.Concrete.ViewProviders
{
    public class ResourcesViewPathRetrieveStrategy : IViewPathRetrieveStrategy
    {
        public string RetrieveViewPath(IView view)
        {
            var path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(view as MonoBehaviour);
                
            int index = path.IndexOf("Resources", StringComparison.Ordinal);

            if (index >= 0)
            {
                // Remove everything before "Resources" and ".prefab" at the end
                return path.Substring(index + "Resources".Length + 1).Replace(".prefab", "");
            }

            throw new Exception("View is located outside of Resources folder");
        }
    }
}
#endif