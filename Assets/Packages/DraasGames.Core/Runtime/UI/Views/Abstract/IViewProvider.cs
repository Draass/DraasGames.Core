using System;
using Cysharp.Threading.Tasks;

namespace DraasGames.Core.Runtime.UI.Views.Abstract
{
    public interface IViewProvider
    {
        /// <summary>
        /// Get view of type T.
        /// </summary>
        /// <typeparam name="T">Target view type</typeparam>
        UniTask<T> GetViewAsync<T>() where T : IViewBase;

        UniTask<IViewBase> GetViewAsync(Type viewType);
    }
}
