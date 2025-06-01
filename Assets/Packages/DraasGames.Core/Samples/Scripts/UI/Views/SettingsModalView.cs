using System;
using DraasGames.Core.Runtime.UI.Views.Concrete;
using UnityEngine;
using UnityEngine.UI;

namespace DraasGames.Core.Samples.Scripts.UI.Views
{
    public class SettingsModalView : View
    {
        public Action OnClose;

        [SerializeField] 
        private Button _closeButton;

        protected override void Awake()
        {
            base.Awake();
            
            _closeButton.onClick.AddListener(() => OnClose.Invoke());
        }
    }
}