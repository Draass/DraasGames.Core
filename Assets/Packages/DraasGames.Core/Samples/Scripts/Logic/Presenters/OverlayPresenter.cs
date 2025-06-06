using Cysharp.Threading.Tasks;
using DraasGames.Core.Runtime.UI.PresenterNavigationService.Abstract;
using DraasGames.Core.Runtime.UI.Views.Abstract;

namespace DraasGames.Core.Samples.Scripts.Logic.Presenters
{
    internal sealed class OverlayPresenter : IPresenter
    {
        private readonly IViewRouter _viewRouter;

        public OverlayPresenter(IViewRouter viewRouter)
        {
            _viewRouter = viewRouter;
        }
        
        public async UniTask ShowAsync()
        {
            var view = await _viewRouter.ShowPersistentAsync<OverlayView>();
        }

        private void DisplayCustomMessage(string message)
        {
            
        }
    }
}