using Cysharp.Threading.Tasks;

namespace DraasGames.Core.Runtime.UI.PresenterNavigationService.Abstract
{
    public interface IPresenter
    {
        UniTask ShowAsync();
    }
    
    public interface IPresenter<in TParam>
    {
        UniTask ShowAsync(TParam param);
    }
}