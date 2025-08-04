using Cysharp.Threading.Tasks;
using DraasGames.Core.Runtime.UI.Views.Concrete;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DraasGames.Core.Samples.Scripts
{
    public class LoadingView : View
    {
        [SerializeField, Required]
        private ViewFadeEffect _viewFadeEffect;


        public override async UniTask ShowAsync()
        {
            await base.ShowAsync();

            await _viewFadeEffect.Show();
        }

        public override async UniTask HideAsync()
        {
            await _viewFadeEffect.Hide();
            
            await base.HideAsync();
        }
    }
}
