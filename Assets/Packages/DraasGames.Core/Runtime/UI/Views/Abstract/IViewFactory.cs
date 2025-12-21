using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace DraasGames.Core.Runtime.UI.Views.Abstract
{
    public interface IViewFactory
    {
        UniTask<IViewBase> Create(Type viewType);

        UniTask<T> Create<T>() where T : MonoBehaviour, IViewBase;
    }
}
