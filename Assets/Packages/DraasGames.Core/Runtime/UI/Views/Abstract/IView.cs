using System;

namespace DraasGames.Core.Runtime.UI.Views.Abstract
{
    public interface IView
    {
        public event Action OnViewShow;
        public event Action OnViewHide;

        public void Show();
        public void Hide();
    }
}