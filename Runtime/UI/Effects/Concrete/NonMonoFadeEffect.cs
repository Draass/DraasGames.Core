#if DRAASGAMES_DOTWEEN_ENABLED
using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DraasGames.Core.Runtime.UI.Effects.Abstract;
using DraasGames.Core.Runtime.UI.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace DraasGames.Core.Runtime.UI.Effects.Concrete
{
    [Serializable]
    public class NonMonoFadeEffect : NonMonoEffect
    {
        [SerializeField] private Graphic _graphic;
        
        [SerializeField, Range(0f, 1f)] 
        private float _toValue = 0f;
        
        [SerializeField] private float _duration = .2f;
        
        public override async UniTask Play(bool reverse = false)
        {
            await _graphic.DOFade(_toValue, _duration).AsyncWaitForCompletion();
        }

        public override void Pause()
        {
            throw new NotImplementedException();
        }

        public override void Stop()
        {
            throw new NotImplementedException();
        }

        public override void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
#endif