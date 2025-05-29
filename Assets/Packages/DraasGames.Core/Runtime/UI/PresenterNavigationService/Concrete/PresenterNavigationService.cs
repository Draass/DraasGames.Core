using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DraasGames.Core.Runtime.UI.PresenterNavigationService.Abstract;
using Zenject;

namespace DraasGames.Core.Runtime.UI.PresenterNavigationService.Concrete
{
    public class PresenterNavigationService : IPresenterNavigationService
    {
        private readonly IInstantiator _instantiator;

        public PresenterNavigationService(IInstantiator instantiator)
        {
            _instantiator = instantiator;
        }
        
        public async UniTask NavigateAsync<TPresenter, TParam>(TParam param) where TPresenter : IPresenter<TParam>
        {
            var presenter = _instantiator.Instantiate<TPresenter>();

            await presenter.ShowAsync(param);
        }

        public async UniTask NavigateAsync<TPresenter>() where TPresenter : IPresenter
        {
            var presenter = _instantiator.Instantiate<TPresenter>();
            
            await presenter.ShowAsync();       
        }
    }
}