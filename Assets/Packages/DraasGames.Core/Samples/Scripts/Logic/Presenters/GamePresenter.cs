using Cysharp.Threading.Tasks;
using DraasGames.Core.Runtime.UI.PresenterNavigationService.Abstract;
using DraasGames.Core.Runtime.UI.Views.Abstract;
using DraasGames.Core.Samples.Scripts.UI.Views;

namespace DraasGames.Core.Samples.Scripts.Logic.Presenters
{
    internal sealed class GamePresenter : IPresenter
    {
        private readonly IViewRouter _viewRouter;
        private readonly IPresenterNavigationService _presenterNavigationService;
        
        public GamePresenter(IViewRouter viewRouter)
        {
            _viewRouter = viewRouter;    
        }

        public async UniTask ShowAsync()
        {
            var view = await _viewRouter.ShowAsync<GameView>();
            
            view.Initialize(OnReturnToMenu);
        }

        private void OnReturnToMenu()
        {
            _presenterNavigationService.NavigateAsync<HubPresenter>();
        }
    }
}