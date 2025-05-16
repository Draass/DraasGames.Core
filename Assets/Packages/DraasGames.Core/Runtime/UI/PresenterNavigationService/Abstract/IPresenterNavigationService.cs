using Cysharp.Threading.Tasks;

namespace DraasGames.Core.Runtime.UI.PresenterNavigationService.Abstract
{
    public interface IPresenterNavigationService
    {
        /// <summary>
        /// Navigates to a specified presenter, passing the provided parameter.
        /// </summary>
        /// <typeparam name="TPresenter">The type of the presenter to navigate to.</typeparam>
        /// <typeparam name="TParam">The type of the parameter to pass to the presenter.</typeparam>
        /// <param name="param">The parameter to pass to the presenter.</param>
        UniTask NavigateAsync<TPresenter, TParam>(TParam param)
            where TPresenter : IPresenter<TParam>;

        /// <summary>
        /// Navigates to a specified presenter.
        /// </summary>
        /// <typeparam name="TPresenter">The type of the presenter to navigate to.</typeparam>
        UniTask NavigateAsync<TPresenter>() where TPresenter : IPresenter;
    }
}