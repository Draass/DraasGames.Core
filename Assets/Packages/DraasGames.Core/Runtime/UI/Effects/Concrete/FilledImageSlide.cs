#if DRAASGAMES_ADDRESSABLES_MODULE
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DraasGames.Runtime.UI.Effects.Abstract;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace DraasGames.Runtime.UI.Effects.Concrete
{
    /// <summary>
    /// Tween image fill amount 
    /// </summary>
    [AddComponentMenu("DraasGames/UI/Effects/FilledImageSlide")]
    public class FilledImageSlide : Effect
    {
        [SerializeField, Required] 
        private Image _image;
        
        [SerializeField, BoxGroup("Parameters"), Tooltip("Initial value of fill amount")] 
        private float _initialValue = 0.0f;
        
        [SerializeField, BoxGroup("Parameters"),Tooltip("Final value of fill amount")] 
        private float _finalValue = 1.0f;
        
        [SerializeField, BoxGroup("Parameters"), Tooltip("Duration of fill amount animation")] 
        private float _duration = 0.5f;

        private Tween _slideTween;

        [Button]
        public override async UniTask Play(bool reverse = false)
        {
            Reset();
            
            _slideTween = DOTween.To(() => _image.fillAmount, x => _image.fillAmount = x, _finalValue, _duration);
            
            await _slideTween.AsyncWaitForCompletion();
            
            // await DOTween
            //     .To(() => _image.fillAmount, x => _image.fillAmount = x, _finalValue, _duration)
            //     .AsyncWaitForCompletion();
        }

        public override void Pause()
        {
            _slideTween?.Pause();
        }

        public override void Stop()
        {
            throw new System.NotImplementedException();
        }

        public void Continue()
        {
            _slideTween?.Play();
        }
        
        public async UniTask Activate(bool instant)
        {
            // Очевидно, что это МЕГАкостыль для онбординга, потому что при скипе панельки не отменяется почему-то твин
            // в идеале - пофиксить, а то так жить нельзя
            DOTween.Complete(_image.fillAmount);
            DOTween.KillAll();
            
            if (instant)
            {
                _image.fillAmount = _finalValue;
            }
            // else
            // {
            //     await Activate();
            // }
        }
        

        public override void Reset()
        {
            // if(_slideTween.active)
            //     _slideTween.Pause();
            
            _image.fillAmount = _initialValue;
            // DOTween.Complete(_image.fillAmount);
            // DOTween.KillAll();
        }
        
        private void OnDestroy()
        {
            DOTween.Kill(_image);
            //_slideTween.Kill();
        }
    }
}
#endif
