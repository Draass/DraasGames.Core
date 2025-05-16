using DraasGames.Core.Runtime.UI.Effects.Abstract;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace DraasGames.Core.Runtime.UI.Extensions
{
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public abstract class CustomSelectable : Selectable
    {
        [SerializeField, BoxGroup("Options"), OnValueChanged(nameof(OnNativeEffectsDisabled))]
        [InfoBox("If true, this UI element will mimic the default Unity Selectable (Button/Toggle) behavior.\n" +
                 "Set to false to apply custom effects for state transitions.")]
        protected bool _isNativeEffects;

        [SerializeReference, BoxGroup("Options/Custom Effects"),
         ShowIf("@_isNativeEffects == false"), Required, Title("Normal Effect"), HideLabel]
        protected NonMonoEffect _normalEffect;

        [SerializeReference, BoxGroup("Options/Custom Effects"),
        ShowIf("@_isNativeEffects == false"), Required, Title("Highlighted Effect"), HideLabel]
        protected NonMonoEffect _highlightedEffect;

        [SerializeReference, BoxGroup("Options/Custom Effects"),
         ShowIf("@_isNativeEffects == false"), Required, Title("Selected Effect"), HideLabel]
        protected NonMonoEffect _selectedEffect;

        [SerializeReference, BoxGroup("Options/Custom Effects"),
         ShowIf("@_isNativeEffects == false"), Required, Title("Pressed Effect"), HideLabel]
        protected NonMonoEffect _pressedEffect;

        [SerializeReference, BoxGroup("Options/Custom Effects"),
         ShowIf("@_isNativeEffects == false"), Required, Title("Disabled Effect"), HideLabel]
        protected NonMonoEffect _disabledEffect;

        [SerializeField, BoxGroup("Options")] protected bool _hasAlphaThreshold;

        [SerializeField, BoxGroup("Options/Alpha Threshold"),
         ShowIf("@_hasAlphaThreshold == true")]
        protected Image _hitImage;

        [SerializeField, BoxGroup("Options/Alpha Threshold"),
         ShowIf("@_hasAlphaThreshold == true"), Range(0f, 1f)]
        protected float _alphaThreshold = 1f;

        protected override void Awake()
        {
            // Ignore base.Awake() because it will try get target graphic and cause issues
            //base.Awake();
            if (_hasAlphaThreshold && _hitImage)
            {
                _hitImage.alphaHitTestMinimumThreshold = _alphaThreshold;
            }
        }

        /// <summary>
        /// Overridden from Unity's Selectable to handle either native transitions or custom effects.
        /// </summary>
        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            if (_isNativeEffects)
            {
                // Use Unity's built-in color / sprite / animation transitions
                base.DoStateTransition(state, instant);
            }
            else
            {
                // Use your custom non-monobehaviour effects
                switch (state)
                {
                    case SelectionState.Normal:
                        _normalEffect?.Play();
                        break;
                    case SelectionState.Highlighted:
                        _highlightedEffect?.Play();
                        break;
                    case SelectionState.Pressed:
                        _pressedEffect?.Play();
                        break;
                    case SelectionState.Selected:
                        _selectedEffect?.Play();
                        break;
                    case SelectionState.Disabled:
                        _disabledEffect?.Play();
                        break;
                }
            }
        }

        /// <summary>
        /// If you want some callback when _isNativeEffects is turned OFF,
        /// you can implement that logic here (like adding default effects, etc.).
        /// </summary>
        protected virtual void OnNativeEffectsDisabled()
        {
            if (_isNativeEffects)
                return;

            // Optionally add default custom effect references if none are assigned,
            // or do any other setup when switching away from native effects.
        }
    }
}