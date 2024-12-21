using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace DraasGames.Core.Runtime.UI.Extensions
{
    [Serializable]
    public abstract class NonMonoEffect
    {
        public abstract UniTask Play(bool reverse = false);
        public abstract void Pause();
        public abstract void Stop();
        public abstract void Reset();
    }

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
    
    /// <summary>
    /// Custom button with option to mimic native button behaviour or override with custom animations
    /// </summary>
    [AddComponentMenu("UI/DraasGames/CustomButton")]
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public class CustomButton : Selectable, IPointerClickHandler, ISubmitHandler
    {
        /// <summary>
        /// Event called on button click or submit
        /// </summary>
        [BoxGroup("Events")]
        public UnityEvent onClick; 
        
        [SerializeField, BoxGroup("Options"), OnValueChanged(nameof(OnNativeEffectsDisabled))] 
        [InfoBox("If true, button will mimic behaviour of default Unity button. Set to false to apply custom effects" +
                 " for state transitions ")]
        private bool _isNativeEffects;

        [SerializeReference, BoxGroup("Options/Custom Effects"), ShowIf("@_isNativeEffects == false"), 
         Required, Title("Normal Effect"), HideLabel]
        private NonMonoEffect _normalEffect;
        
        [FormerlySerializedAs("_hoverEffect")]
        [SerializeReference, BoxGroup("Options/Custom Effects"), ShowIf("@_isNativeEffects == false"), 
         Required, Title("Highlighted Effect"), HideLabel]
        private NonMonoEffect _highlightedEffect;

        [SerializeReference, BoxGroup("Options/Custom Effects"), ShowIf("@_isNativeEffects == false"), 
         Required, Title("Selected Effect"), HideLabel]
        private NonMonoEffect _selectedEffect;
        
        [SerializeReference, BoxGroup("Options/Custom Effects"), ShowIf("@_isNativeEffects == false"), 
         Required, Title("Pressed Effect"), HideLabel]
        private NonMonoEffect _pressedEffect;

        [SerializeReference, BoxGroup("Options/Custom Effects"), ShowIf("@_isNativeEffects == false"), 
         Required, Title("Disabled Effect"), HideLabel]
        private NonMonoEffect _disabledEffect;

        [SerializeField, BoxGroup("Options")]
        private bool _hasAlphaThreshold;
        
        [SerializeField, BoxGroup("Options"), ShowIf("@_hasAlphaThreshold == true")]
        private Image _hitImage;
        
        [SerializeField, BoxGroup("Options"), ShowIf("@_hasAlphaThreshold == true"), Range(0f, 1f)]
        private float _alphaThreshold = 1f;
        
        private Button _button;
        private Toggle _toggle;

        private new void Awake()
        {
            if(_hasAlphaThreshold)
            {
                _hitImage.alphaHitTestMinimumThreshold = _alphaThreshold;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(!IsInteractable() || !IsActive())
                return;
            
            onClick.Invoke();
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if(!IsInteractable() || !IsActive())
                return;
            
            onClick.Invoke();
        }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            if(_isNativeEffects)
                base.DoStateTransition(state, instant);
            else
            {
                switch (state)
                {
                    case SelectionState.Normal:
                        _normalEffect.Play();
                        break;
                    case SelectionState.Highlighted:
                        _highlightedEffect.Play();
                        break;
                    case SelectionState.Pressed:
                        _pressedEffect.Play();
                        break;
                    case SelectionState.Selected:
                        _selectedEffect.Play();
                        break;
                    case SelectionState.Disabled:
                        _disabledEffect.Play();
                        break;
                }
            }
        }

        private void OnNativeEffectsDisabled()
        {
            if(_isNativeEffects)
                return;
            
            // TODO Add some default effects? dunno
        }
    }
}