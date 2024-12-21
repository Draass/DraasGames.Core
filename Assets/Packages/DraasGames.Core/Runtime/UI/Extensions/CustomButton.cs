using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DraasGames.Core.Runtime.UI.Extensions
{
    /// <summary>
    /// Custom button with option to mimic native button behaviour or override with custom animations
    /// </summary>
    [AddComponentMenu("UI/DraasGames/CustomButton")]
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public class CustomButton : CustomSelectable, IPointerClickHandler, ISubmitHandler
    {
        public event Action OnClick; 

        public void OnPointerClick(PointerEventData eventData)
        {
            if(!IsInteractable() || !IsActive())
                return;
            
            OnClick?.Invoke();
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if(!IsInteractable() || !IsActive())
                return;
            
            OnClick?.Invoke();
        }
    }
}