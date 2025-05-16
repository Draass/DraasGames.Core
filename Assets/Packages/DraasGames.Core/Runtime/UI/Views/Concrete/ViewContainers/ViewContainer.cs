using System;
using DraasGames.Core.Runtime.UI.Views.Abstract;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DraasGames.Core.Runtime.UI.Views.Concrete.ViewContainers
{
    [Obsolete]
    public abstract class ViewContainer : SerializedScriptableObject
    {
        public abstract string GetViewPath<T>() where T : MonoBehaviour, IView;
        public abstract string GetViewPath(Type viewType);
        public abstract void AddView(IView view);
    }
}