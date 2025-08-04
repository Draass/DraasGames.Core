using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace DraasGames.Core.Samples.Scripts
{
    public class ViewFadeEffect : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup _canvasGroup;
        
        [SerializeField]
        private float _duration = 1;
        
        public async UniTask Show()
        {
            _canvasGroup.alpha = 0;
            
            await _canvasGroup.DOFade(1, _duration).AsyncWaitForCompletion();
        }

        public async UniTask Hide()
        {
            _canvasGroup.alpha = 1;
            
            await _canvasGroup.DOFade(0, _duration).AsyncWaitForCompletion();
        }
    }
}