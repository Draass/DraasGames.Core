using System;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DraasGames.Core.Runtime.UI.Extensions
{
    [CustomEditor(typeof(CustomToggle))]
    public class CustomToggleOdinEditor : OdinEditor
    {
    }
    
    [AddComponentMenu("UI/DraasGames/CustomToggle")]
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public class CustomToggle : Selectable, IPointerClickHandler, ISubmitHandler
    {
        public enum SwitchEffect
        {
            None,
            Custom
        }

        public enum ToggleMode
        {
            ToggleSingle,
            ToggleBetween,
            Custom
        }
        
        public event Action<bool> OnValueChanged;
        
        [ShowInInspector, ReadOnly, BoxGroup("Debug")]
        public bool IsOn { get; private set; }

        [SerializeField, BoxGroup("Options")]
        private bool _isNativeEffects;

        [SerializeReference, BoxGroup("Options/Custom Effects"), ShowIf("@_isNativeEffects == false"), 
         Required, Title("Normal Effect"), HideLabel]
        private NonMonoEffect _normalEffect;

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

        [SerializeReference, BoxGroup("Options/Custom Effects"), ShowIf("@_isNativeEffects == false && _switchEffect == SwitchEffect.Custom"), 
         Required, Title("Toggle Active Switch Effect"), HideLabel]
        private NonMonoEffect _toggleSwitchEffect;
        
        [SerializeReference, BoxGroup("Options/Custom Effects"), ShowIf("@_isNativeEffects == false && _switchEffect == SwitchEffect.Custom"), 
         Required, Title("Toggle InActive Switch Effect"), HideLabel]
        private NonMonoEffect _toggleInactiveSwitchEffect;
        
        [SerializeField, ShowIf("@_toggleMode == ToggleMode.ToggleSingle || _toggleMode == ToggleMode.ToggleBetween")]
        private GameObject _activeGraphic;
        
        [SerializeField, ShowIf("@_toggleMode == ToggleMode.ToggleBetween")]
        private GameObject _inactiveGraphic;

        [SerializeField]
        private SwitchEffect _switchEffect;
        
        [SerializeField, BoxGroup("Options"), HideIf("_isNativeEffects")] 
        private ToggleMode _toggleMode;
        
        [SerializeField, BoxGroup("Options")]
        private bool _hasAlphaThreshold;
        
        [SerializeField, BoxGroup("Options/Alpha Threshold"), ShowIf("@_hasAlphaThreshold == true")]
        private Image _hitImage;
        
        [SerializeField, BoxGroup("Options/Alpha Threshold"), ShowIf("@_hasAlphaThreshold == true"), Range(0f, 1f)]
        private float _alphaThreshold = 1f;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!IsInteractable() || !IsActive())
                return;
            
            SetIsOn(!IsOn);
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (!IsInteractable() || !IsActive())
                return;
            
            SetIsOn(!IsOn);
        }
        
        public void SetIsOnWithoutNotify(bool value)
        {
            IsOn = value;
            UpdateGraphic();
        }
        
        public void SetIsOn(bool value)
        {
            IsOn = value;
            OnValueChanged?.Invoke(value);
            UpdateGraphic();
        }

        private void UpdateGraphic()
        {
            switch (_toggleMode)
            {
                case ToggleMode.ToggleSingle:
                    _activeGraphic?.SetActive(IsOn);
                    break;
                case ToggleMode.ToggleBetween:
                    _activeGraphic?.SetActive(IsOn);
                    _inactiveGraphic?.SetActive(!IsOn);
                    break;
                case ToggleMode.Custom:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}