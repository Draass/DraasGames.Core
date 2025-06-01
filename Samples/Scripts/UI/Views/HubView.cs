using System;
using DraasGames.Core.Runtime.UI.Views.Concrete;
using UnityEngine;
using UnityEngine.UI;

namespace DraasGames.Core.Samples.Scripts
{
    public record HubModel
    {
        public Action OnShowSettings;
    }
    
    public class HubView : View
    {
        [SerializeField] 
        private Button _showSettingsButton;

        private Action _onShowSettings;
        
        public void Initialize(Action onShowSettings)
        {
            _onShowSettings = onShowSettings;
        }

        protected override void Awake()
        {
            base.Awake();
            
            _showSettingsButton.onClick.AddListener(() => _onShowSettings?.Invoke());
        }
    }
}