using System;
using Cysharp.Threading.Tasks;
using DraasGames.Core.Runtime.UI.Views.Concrete;
using Sirenix.OdinInspector;
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

        [SerializeField, Required]
        private ViewFadeEffect _viewFadeEffect;
        
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
        
        public override async UniTask ShowAsync()
        {
            await base.ShowAsync();

            await _viewFadeEffect.Show();
        }

        public override async UniTask HideAsync()
        {
            await _viewFadeEffect.Hide();
            
            await base.HideAsync();
        }
    }
}