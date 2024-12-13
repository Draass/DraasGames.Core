#if DRAASGAMES_ADDRESSABLES_MODULE
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DraasGames.Runtime.UI.Effects.Abstract;
using Sirenix.OdinInspector;
using UnityEngine;

namespace DraasGames.Runtime.UI.Effects.Concrete
{
    public class MoveBetweenEffect : Effect
    {
        [SerializeField, Required] private RectTransform _from;
        [SerializeField, Required] private RectTransform _to;
        [SerializeField, Required] private RectTransform _target;
        [SerializeField] private float _duration;
        [SerializeField] private bool _activateOnAwake;
        [SerializeField] private Ease _ease = Ease.Linear;

        private Tween _tween;
        
        private void Awake()
        {
            if (_activateOnAwake)
                Play().Forget();
        }

        [Button]
        public override async UniTask Play(bool reverse = false)
        {
            _tween = _target
                .DOAnchorPosX(_to.anchoredPosition.x, _duration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(_ease);
            
            _tween.Play();
        }

        public override void Pause()
        {
            throw new System.NotImplementedException();
        }

        public override void Stop()
        {
            throw new System.NotImplementedException();
        }

        public override void Reset()
        {
            throw new System.NotImplementedException();
        }

        private void OnDestroy()
        {
            DOTween.Kill(_tween);
        }
    }
}
#endif