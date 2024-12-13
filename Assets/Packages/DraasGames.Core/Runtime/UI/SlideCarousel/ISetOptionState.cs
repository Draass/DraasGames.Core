using UnityEngine;

namespace DraasGames.Core.Runtime.UI.SlideCarousel
{
    public interface ISetOptionState
    {
        public void SetOptionState(bool isActive);
        
        public GameObject GameObject { get; }
    }
}