using System;
using Cysharp.Threading.Tasks;
using DraasGames.Core.Runtime.UI.PresenterNavigationService.Abstract;
using DraasGames.Core.Runtime.UI.Views.Abstract;
using DraasGames.Core.Samples.Scripts.Logic.Presenters;
using UnityEngine;
using Zenject;

namespace DraasGames.Core.Samples.Scripts
{
    public class Bootstrap : MonoBehaviour
    {
        private IViewRouter _viewRouter;
        private IPresenterNavigationService _presenterNavigationService;
        
        [Inject]
        private void Construct(
            IViewRouter viewRouter,
            IPresenterNavigationService presenterNavigationService)
        {
            _viewRouter = viewRouter;
            _presenterNavigationService = presenterNavigationService;
        }

        private void Start()
        {
            ImitateLoading().Forget();
        }

        private async UniTaskVoid ImitateLoading()
        {
            _viewRouter.Show<LoadingView>();

            await UniTask.Delay(TimeSpan.FromSeconds(3f));

            await _presenterNavigationService.NavigateAsync<HubPresenter>();
        }
    }
}
