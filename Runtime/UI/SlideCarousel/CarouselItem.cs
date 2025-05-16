using UnityEngine;

namespace DraasGames.Core.Runtime.UI.SlideCarousel
{
    public class CarouselItem : MonoBehaviour, ISetOptionState
    {
        [SerializeField] private GameObject activePanel;
        [SerializeField] private GameObject inactivePanel;

        public GameObject GameObject => gameObject;
        
        public void SetOptionState(bool isActive)
        {
            return;
            
            if (isActive)
            {
                inactivePanel.SetActive(false);
                activePanel.SetActive(true);
            }
            else
            {
                activePanel.SetActive(false);
                inactivePanel.SetActive(true);
            }
        }
    }

    // TODO
    // public class BaseCarouselItem : MonoBehaviour
    // {
    //     public bool IsSelected { get; private set; }
    //     
    //     public virtual void Select() => IsSelected = true;
    //     public virtual void Deselect() => IsSelected = false;
    // }
}