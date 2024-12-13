using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace DraasGames.Core.Runtime.UI.Views.Abstract
{
    public interface IViewFactory
    {
        public UniTask<IView> Create(Type viewType);
        
        public UniTask<T> Create<T>() where T : MonoBehaviour, IView;
    }
}