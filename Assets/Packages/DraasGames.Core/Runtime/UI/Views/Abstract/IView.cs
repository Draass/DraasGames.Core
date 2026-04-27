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

    public interface ICacheableView : IViewBase
    {
        UniTask ResetStateAsync() => UniTask.CompletedTask;
    }

    public interface IDestroyableView : IViewBase
    {
        void DestroyView();
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
