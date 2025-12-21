using System;
using Cysharp.Threading.Tasks;

namespace DraasGames.Core.Runtime.UI.Views.Abstract
{
    public interface IViewBase
    {
        event Action OnViewShow;
        event Action OnViewHide;

        UniTask HideAsync();
    }

    public interface IView : IViewBase
    {
        UniTask ShowAsync();
    }

    public interface IView<in TParam> : IViewBase
    {
        UniTask ShowAsync(TParam param);
    }
}
