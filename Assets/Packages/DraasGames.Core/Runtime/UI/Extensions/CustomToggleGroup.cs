using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DraasGames.Core.Runtime.UI.Extensions
{
    public class CustomToggleGroup : UIBehaviour
    {
        [SerializeField] private CustomToggle[] _toggles = Array.Empty<CustomToggle>();
        [SerializeField] private bool _allowSwitchOff = false;

        [Tooltip("How many toggles can be active at the same time. -1 for unlimited.")] [SerializeField]
        private int _maxTogglesActiveAllowed = -1;

        private readonly List<CustomToggle> _activeToggles = new();
        private readonly Dictionary<CustomToggle, Action<bool>> _subscriptions = new();
        private bool _isHandling;

        /// <summary>Read-only list of active toggles.</summary>
        public IReadOnlyList<CustomToggle> ActiveToggles => _activeToggles;

        /// <summary>Group-level event fired after rules applied.</summary>
        public event Action<IReadOnlyList<CustomToggle>> GroupStateChanged;

        protected override void OnEnable()
        {
            base.OnEnable();
            Subscribe();
            SyncInitialState();
        }

        protected override void OnDisable()
        {
            Unsubscribe();
            _activeToggles.Clear();
            base.OnDisable();
        }

        private void Subscribe()
        {
            if (_toggles == null) return;
            foreach (var t in _toggles)
            {
                if (!t) continue;
                var tt = t; // capture
                Action<bool> handler = v => OnToggleValueChanged(tt, v);
                tt.OnValueChanged += handler;
                _subscriptions[tt] = handler;
            }
        }

        private void Unsubscribe()
        {
            foreach (var kv in _subscriptions)
            {
                if (kv.Key) kv.Key.OnValueChanged -= kv.Value;
            }

            _subscriptions.Clear();
        }

        private void OnToggleValueChanged(CustomToggle sender, bool value)
        {
            if (_isHandling) return; // guard against reentry
            _isHandling = true;

            if (value)
            {
                if (!_activeToggles.Contains(sender))
                    _activeToggles.Add(sender);

                // Radio-like behaviour
                if (!_allowSwitchOff || _maxTogglesActiveAllowed == 1)
                {
                    for (int i = _activeToggles.Count - 1; i >= 0; i--)
                    {
                        var t = _activeToggles[i];
                        if (t == sender) continue;
                        t.SetIsOnWithoutNotify(false);
                        _activeToggles.RemoveAt(i);
                    }
                }

                // Enforce max active (if not unlimited and >1)
                if (_maxTogglesActiveAllowed != -1 && _activeToggles.Count > _maxTogglesActiveAllowed)
                {
                    // Remove oldest (except sender if possible)
                    for (int i = 0; i < _activeToggles.Count && _activeToggles.Count > _maxTogglesActiveAllowed; i++)
                    {
                        var t = _activeToggles[i];
                        if (t == sender) continue;
                        t.SetIsOnWithoutNotify(false);
                        _activeToggles.RemoveAt(i);
                        i--; // adjust index after removal
                    }

                    // If still over limit (all others were sender), trim from start
                    while (_activeToggles.Count > _maxTogglesActiveAllowed)
                    {
                        var t = _activeToggles[0];
                        if (t != sender) t.SetIsOnWithoutNotify(false);
                        _activeToggles.RemoveAt(0);
                    }
                }
            }
            else // value == false
            {
                _activeToggles.Remove(sender);

                if (!_allowSwitchOff && _activeToggles.Count == 0)
                {
                    // force at least one active: re-enable sender
                    sender.SetIsOnWithoutNotify(true);
                    _activeToggles.Add(sender);
                }
            }

            _isHandling = false;
            GroupStateChanged?.Invoke(_activeToggles);
        }

        private void SyncInitialState()
        {
            _activeToggles.Clear();
            if (_toggles != null)
            {
                foreach (var t in _toggles)
                {
                    if (!t) continue;
                    if (t.IsOn) _activeToggles.Add(t);
                }
            }

            // Ensure at least one active if not allowed to switch all off
            if (!_allowSwitchOff && _activeToggles.Count == 0 && _toggles.Length > 0)
            {
                var first = _toggles[0];
                if (first)
                {
                    first.SetIsOnWithoutNotify(true);
                    _activeToggles.Add(first);
                }
            }

            // Enforce max
            if (_maxTogglesActiveAllowed != -1 && _activeToggles.Count > _maxTogglesActiveAllowed)
            {
                for (int i = _maxTogglesActiveAllowed; i < _activeToggles.Count; i++)
                {
                    var t = _activeToggles[i];
                    t.SetIsOnWithoutNotify(false);
                }

                _activeToggles.RemoveRange(_maxTogglesActiveAllowed, _activeToggles.Count - _maxTogglesActiveAllowed);
            }

            GroupStateChanged?.Invoke(_activeToggles);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (_maxTogglesActiveAllowed < -1) _maxTogglesActiveAllowed = -1;
            if (_toggles == null || _toggles.Length == 0)
                _toggles = GetComponentsInChildren<CustomToggle>(true);
        }
#endif
    }
}