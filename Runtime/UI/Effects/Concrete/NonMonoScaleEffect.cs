#if DRAASGAMES_EFFECTS_MODULE
using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DraasGames.Core.Runtime.UI.Effects.Abstract;
using DraasGames.Core.Runtime.UI.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DraasGames.Core.Runtime.UI.Effects.Concrete
{
    [Serializable]
    public class NonMonoScaleEffect : NonMonoEffect
    {
        [SerializeField, Required] private RectTransform _target;
        [SerializeField] private float _targetScale = 1.5f;
        [SerializeField] private float _duration = .1f;
        [SerializeField] private Ease _ease = Ease.Linear;
        
        private Tween _scaleTween;
        
        public override async UniTask Play(bool reverse = false)
        {
            _scaleTween = _target
                .DOScale(_targetScale, _duration)
                .SetEase(_ease);
            
            _scaleTween.Play();
        }

        public override void Pause()
        {
                     
        }

        public override void Stop()
        {
            
        }

        public override void Reset()
        {
            
        }
    }
}
#endif