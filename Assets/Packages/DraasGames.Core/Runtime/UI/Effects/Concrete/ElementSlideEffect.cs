#if DRAASGAMES_EFFECTS_MODULE
using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DraasGames.Core.Runtime.UI.Effects.Abstract;
using DraasGames.Core.Runtime.UI.Effects.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DraasGames.Core.Runtime.UI.Effects.Concrete
{
    [AddComponentMenu("DraasGames/UI/Effects/ElementSlideEffect")]
    public class ElementSlideEffect : Effect
    {
        [SerializeField, Required] private RectTransform _target;
        [SerializeField] private SlideDirection _slideDirection = SlideDirection.RightToLeft;
        [SerializeField, Range(0f, 2f)] private float _duration = 1f;

        [Tooltip("How far will element slide? By default it sides on full element axis size")]
        [SerializeField] private float _slideFactor = 1f;

        [ShowInInspector] private float posXInitial;
        [ShowInInspector] private float posYInitial;
        
        private void Start()
        {
            posXInitial = _target.anchoredPosition.x;
            posYInitial = _target.anchoredPosition.y;
        }
        
        [Button]
        public override async UniTask Play(bool reverse = false)
        {
            //BUG If scene is additively loaded, pos initial is 0
            if(posXInitial == 0)
                posXInitial = _target.position.x;
            
            switch (_slideDirection)
            {
                case SlideDirection.LeftToRight:
                    _target.DOAnchorPosX(posXInitial + _target.rect.width * _slideFactor, _duration);
                    break;
                case SlideDirection.RightToLeft:
                    _target.DOAnchorPosX(posXInitial - _target.rect.width * _slideFactor, _duration);
                    break;
                case SlideDirection.UpToDown:
                    _target.DOAnchorPosY(posYInitial - _target.rect.height * _slideFactor, _duration);
                    break;
                case SlideDirection.DownToUp:
                    _target.DOAnchorPosY(posYInitial + _target.rect.height * _slideFactor, _duration);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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
            _target.DOAnchorPosX(posXInitial, 0);
        }

        [Button]
        public async UniTask Reverse()
        {
            switch (_slideDirection)
            {
                case SlideDirection.LeftToRight:
                    _target.DOAnchorPosX(posXInitial, _duration);
                    break;
                case SlideDirection.RightToLeft:
                    _target.DOAnchorPosX(posXInitial, _duration);
                    break;
                case SlideDirection.UpToDown:
                    _target.DOAnchorPosY(posYInitial, _duration);
                    break;
                case SlideDirection.DownToUp:
                    _target.DOAnchorPosY(posYInitial, _duration);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
#endif