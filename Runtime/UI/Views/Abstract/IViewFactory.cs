using System;
using Cysharp.Threading.Tasks;

namespace DraasGames.Core.Runtime.UI.Views.Abstract
{
    public interface IViewFactory
    {
        UniTask<IViewBase> Create(Type viewType);

        UniTask<T> Create<T>() where T : IViewBase;
    }
}
