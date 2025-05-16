﻿using System;
using DraasGames.Core.Runtime.UI.Effects.Abstract;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DraasGames.Core.Runtime.UI.Extensions
{
    [AddComponentMenu("DraasGames/UI/CustomToggle")]
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public class CustomToggle : CustomSelectable, IPointerClickHandler, ISubmitHandler
    {
        private enum SwitchEffect
        {
            None,
            Custom
        }

        private enum ToggleMode
        {
            ToggleSingle,
            ToggleBetween
        }
        
        public event Action<bool> OnValueChanged;
        
        [ShowInInspector, ReadOnly, BoxGroup("Debug")]
        public bool IsOn { get; private set; }
        
        [SerializeField, BoxGroup("Options")] 
        [Tooltip("Toggle disables single object or switches between two objects")]
        private ToggleMode _toggleMode;
        
        [SerializeField, ShowIf("@_toggleMode == ToggleMode.ToggleSingle || _toggleMode == ToggleMode.ToggleBetween"),
         BoxGroup("Options/Toggle")]
        private GameObject _activeGraphic;
        
        [SerializeField, ShowIf("@_toggleMode == ToggleMode.ToggleBetween"),
         BoxGroup("Options/Toggle")]
        private GameObject _inactiveGraphic;

        [SerializeField, BoxGroup("Options")]
        private SwitchEffect _switchEffect;
        
        [SerializeReference, BoxGroup("Options/Switch Mode"), 
         ShowIf("@_switchEffect == SwitchEffect.Custom"), 
         Required, Title("Toggle Active Switch Effect"), HideLabel]
        private NonMonoEffect _toggleSwitchEffect;
        
        [SerializeReference, BoxGroup("Options/Switch Mode"), 
         ShowIf("@_switchEffect == SwitchEffect.Custom"), 
         Required, Title("Toggle InActive Switch Effect"), HideLabel]
        private NonMonoEffect _toggleInactiveSwitchEffect;
    
        public void OnPointerClick(PointerEventData eventData)
        {
            if(!IsInteractable() || !IsActive())
                return;
            
            SetIsOn(!IsOn);
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if(!IsInteractable() || !IsActive())
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

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            if (_isNativeEffects)
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

        private void UpdateGraphic()
        {
            switch (_toggleMode)
            {
                case ToggleMode.ToggleSingle:
                    ToggleSingeOnEffect();
                    break;
                case ToggleMode.ToggleBetween:
                    ToggleBetweenOnEffect();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void ToggleSingeOnEffect()
        {
            if (_switchEffect == SwitchEffect.Custom)
            {
                if (IsOn)
                {
                    _toggleSwitchEffect.Play();
                }
                else
                {
                    _toggleInactiveSwitchEffect.Play();
                }
            }
            else if (_switchEffect == SwitchEffect.None)
            {
                _activeGraphic?.SetActive(IsOn);
            }
        }
        
        private void ToggleBetweenOnEffect()
        {
            if(_switchEffect == SwitchEffect.Custom)
            {
                if (IsOn)
                {
                    _toggleSwitchEffect.Play();
                }
                else
                {
                    _toggleInactiveSwitchEffect.Play();
                }
            }
            else if (_switchEffect == SwitchEffect.None)
            {
                _activeGraphic?.SetActive(IsOn);
                _inactiveGraphic?.SetActive(!IsOn);
            }
        }
    }
}