using System;
using Cysharp.Threading.Tasks;
using DraasGames.Core.Runtime.UI.Views.Abstract;
using UnityEngine;
using Zenject;

namespace DraasGames.Core.Samples.Scripts
{
    public class Bootstrap : MonoBehaviour
    {
        private IViewRouter _viewRouter;
        
        [Inject]
        private void Construct(IViewRouter viewRouter)
        {
            _viewRouter = viewRouter;
        }

        private void Start()
        {
            ImitateLoading().Forget();
        }

        private async UniTaskVoid ImitateLoading()
        {
            _viewRouter.Show<LoadingView>();

            await UniTask.Delay(TimeSpan.FromSeconds(3f));
            
            _viewRouter.Show<MainView>();
        }
    }
}
