using System;
using DraasGames.Core.Runtime.UI.Views.Concrete;
using UnityEngine;
using UnityEngine.UI;

namespace DraasGames.Core.Samples.Scripts.UI.Views
{
    internal sealed class GameView : View
    {
        [SerializeField]
        private Button _returnToMenuButton;
        
        private Action _onReturnToMenu;

        protected override void Awake()
        {
            base.Awake();
            
            _returnToMenuButton.onClick.AddListener(() => _onReturnToMenu?.Invoke());
        }
        
        public void Initialize(Action onReturnToMenu)
        {
            _onReturnToMenu = onReturnToMenu;
        }
    }
}