using Cysharp.Threading.Tasks;
using DraasGames.Core.Runtime.UI.PresenterNavigationService.Abstract;
using DraasGames.Core.Runtime.UI.Views.Abstract;
using DraasGames.Core.Samples.Scripts.UI.Views;

namespace DraasGames.Core.Samples.Scripts.Logic.Presenters
{
    internal sealed class SettingsPresenter : IPresenter
    {
        private readonly IViewRouter _viewRouter;
        
        public async UniTask ShowAsync()
        {
            var view = await _viewRouter.ShowModalAsync<SettingsModalView>();

            view.OnClose = CloseView;
        }

        private void CloseView()
        {
            _viewRouter.Hide<SettingsModalView>();
        }
    }
}