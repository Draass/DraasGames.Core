using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace DraasGames.Core.Runtime.UI.Views.Abstract
{
    public interface IViewProviderAsync
    {
        /// <summary>
        /// Get view of type T
        /// </summary>
        /// <typeparam name="T">Target view type</typeparam>
        /// <returns></returns>
        public UniTask<T> GetViewAsync<T>() where T : MonoBehaviour, IView;
        
        public UniTask<IView> GetViewAsync(Type viewType);
    }
}