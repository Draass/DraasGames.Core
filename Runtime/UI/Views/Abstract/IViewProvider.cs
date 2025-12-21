using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace DraasGames.Core.Runtime.UI.Views.Abstract
{
    public interface IViewProvider
    {
        /// <summary>
        /// Get view of type T.
        /// </summary>
        /// <typeparam name="T">Target view type</typeparam>
        UniTask<T> GetViewAsync<T>() where T : MonoBehaviour, IViewBase;

        UniTask<IViewBase> GetViewAsync(Type viewType);
    }
}
