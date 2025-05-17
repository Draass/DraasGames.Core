using Cysharp.Threading.Tasks;
using DraasGames.Core.Runtime.Infrastructure.Logger;
using DraasGames.Core.Runtime.UI.PresenterNavigationService.Abstract;
using DraasGames.Core.Runtime.UI.Views.Abstract;

namespace DraasGames.Core.Samples.Scripts.Logic.Presenters
{
    internal sealed class LanguageSettingsPresenter : IPresenter
    {
        private readonly IViewRouter _viewRouter;
        
        public async UniTask ShowAsync()
        {
            var view = await _viewRouter.ShowModalAsync<LanguageSettingsView>(false);
            
            view.OnClose = OnClose;
            view.OnReturnToMenu = OnReturnToMenu;
            view.OnSwitchLanguage = OnSwitchLanguage;
        }
        
        private void OnClose()
        {
            _viewRouter.Hide<LanguageSettingsView>();
        }
        
        private void OnReturnToMenu()
        {
            _viewRouter.HideAllModalViews();
        }
        
        private void OnSwitchLanguage()
        {
            DLogger.Log("Sent command to switch language", this);
        }
    }
}