using System;
using DraasGames.Core.Runtime.UI.Views.Concrete;
using UnityEngine;
using UnityEngine.UI;

namespace DraasGames.Core.Samples.Scripts.UI.Views
{
    internal class LanguageSettingsView : View
    {
        [SerializeField] 
        private Button _closeButton;

        [SerializeField] 
        private Button _returnToMenuButton;
        
        [SerializeField]
        private Button _switchLanguageButton;
        
        public Action OnClose;
        public Action OnReturnToMenu;
        public Action OnSwitchLanguage;

        protected override void Awake()
        {
            base.Awake();
            
            _closeButton.onClick.AddListener(() => OnClose?.Invoke());
            _returnToMenuButton.onClick.AddListener(() => OnReturnToMenu?.Invoke());
            _switchLanguageButton.onClick.AddListener(() => OnSwitchLanguage?.Invoke());
        }
    }
}