using Cysharp.Threading.Tasks;
using DraasGames.Core.Runtime.UI.PresenterNavigationService.Abstract;
using DraasGames.Core.Runtime.UI.Views.Abstract;

namespace DraasGames.Core.Samples.Scripts.Logic.Presenters
{
    internal sealed class CustomMessagePresenter : IPresenter<string>
    {
        private readonly IViewRouter _viewRouter;

        public CustomMessagePresenter(IViewRouter viewRouter)
        {
            _viewRouter = viewRouter;
        }
        
        public async UniTask ShowAsync(string message)
        {
            var view = await _viewRouter.ShowModalAsync<CustomMessageView>();
            
            view.Initialize(message);
        }
    }
}