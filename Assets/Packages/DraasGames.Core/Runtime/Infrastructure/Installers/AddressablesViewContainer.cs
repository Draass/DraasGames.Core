using System;
using System.Collections.Generic;
using DraasGames.Core.Runtime.UI.Views.Concrete;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace DraasGames.Core.Runtime.Infrastructure.Installers
{
    [CreateAssetMenu(menuName = "DraasGames/UI/AddressablesViewContainer", fileName = "AddressablesViewContainer", order = 0)]
    public class AddressablesViewContainer : SerializedScriptableObject, IViewAssetReferenceProvider
    {
        [SerializeField] 
        private Dictionary<Type, AssetReference> _views = new();
        
        public AssetReference GetAssetReference(Type type)
        {
            return _views.GetValueOrDefault(type);
        }
        
#if UNITY_EDITOR
        [Button, BoxGroup("Editor")]
        private void AddView(AssetReferenceT<View> view)
        {
            _views.TryAdd(view.Asset.GetType(), view);
        }  

        [SerializeReference, BoxGroup("Editor/Multiple")]
        private List<AssetReference> _viewsToAdd = new();
        
        [Button, BoxGroup("Editor/Multiple")]
        private void AddViews()
        {
            foreach (var view in _viewsToAdd)
            {
                var assetGo = view.editorAsset as GameObject;
                
                _views.TryAdd(assetGo.GetComponent<View>().GetType(), view);
            }
        }

        [Button, BoxGroup("Editor")]
        private void Clear()
        {
            _views.Clear();
        }
#endif
    }
}