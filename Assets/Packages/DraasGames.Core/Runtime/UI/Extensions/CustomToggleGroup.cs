using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DraasGames.Core.Runtime.UI.Extensions
{
    public class CustomToggleGroup : UIBehaviour
    {
        [SerializeField] private CustomToggle[] _toggles;
        
        [SerializeField] private bool _allowSwitchOff = true;
        
        [Tooltip("How many toggles can be active at the same time. -1 for unlimited")]
        [SerializeField] private int _maxTogglesActiveAllowed = -1;
        
        private Queue<CustomToggle> _activeToggles = new();

        protected override void OnEnable()
        {
            base.OnEnable();
            
            foreach (var toggle in _toggles)
            {
                toggle.OnValueChanged += (OnToggleValueChanged);
                _activeToggles.Enqueue(toggle);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }
        
        public void OnToggleValueChanged(bool value)
        {
            if (_activeToggles.Count >= _maxTogglesActiveAllowed && _maxTogglesActiveAllowed != -1)
            {
                Debug.LogWarning("Max toggles active allowed reached");
                return;
            }
            
            foreach (var toggle in _activeToggles)
            {
                if (toggle.IsOn == value)
                    continue;
                
                toggle.SetIsOn(value);
            }
            
            //_activeToggles.Enqueue();
        }
    }
}