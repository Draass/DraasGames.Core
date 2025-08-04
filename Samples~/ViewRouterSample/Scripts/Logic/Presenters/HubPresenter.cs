using Cysharp.Threading.Tasks;
using DraasGames.Core.Runtime.UI.PresenterNavigationService.Abstract;
using DraasGames.Core.Runtime.UI.Views.Abstract;

namespace DraasGames.Core.Samples.Scripts.Logic.Presenters
{
    internal sealed class HubPresenter : IPresenter
    {
        private readonly IViewRouter _viewRouter;
        private readonly IPresenterNavigationService _presenterNavigationService;

        public HubPresenter(
            IViewRouter viewRouter,
            IPresenterNavigationService presenterNavigationService)
        {
            _viewRouter = viewRouter;
            _presenterNavigationService = presenterNavigationService;
        }
        
        public async UniTask ShowAsync()
        {
            var view = await _viewRouter.ShowAsync<HubView>(ViewTransitionMode.Simultaneous);
            
            view.Initialize(OnShowSettings);
        }

        private void OnShowSettings()
        {
            _presenterNavigationService.NavigateAsync<SettingsPresenter>().Forget();
        }
    }
}