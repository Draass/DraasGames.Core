using System;
using Cysharp.Threading.Tasks;

namespace DraasGames.Core.Runtime.UI.Views.Abstract
{
    public interface IView
    {
        public event Action OnViewShow;
        public event Action OnViewHide;

        [Obsolete("This API is deprecated and is replaced with async overload. " +
                  "Consider using ShowAsync instead.")]
        public void Show();
        
        public UniTask ShowAsync();

        [Obsolete("This API is deprecated and is replaced with async overload. " +
                  "Consider using HideAsync instead.")]
        public void Hide();

        public UniTask HideAsync();
    }
}