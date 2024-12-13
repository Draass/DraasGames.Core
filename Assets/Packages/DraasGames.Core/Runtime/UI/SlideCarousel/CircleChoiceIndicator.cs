using UnityEngine;
using UnityEngine.Serialization;

namespace DraasGames.Core.Runtime.UI.SlideCarousel
{
    public class CircleChoiceIndicator : MonoBehaviour
    {
        [FormerlySerializedAs("_carouselr")] [FormerlySerializedAs("carousel")] [SerializeField] private Carousel _carousel;
        [SerializeField] private Transform activeCircle;
        
        private void OnEnable()
        {
            _carousel.OnActivityChanged += CarouselOnOnActivityChanged;
        }
        
        private void OnDisable()
        {
            _carousel.OnActivityChanged -= CarouselOnOnActivityChanged;
        }
        
        private void CarouselOnOnActivityChanged(int value)
        {
            activeCircle.SetSiblingIndex(value);
        }
    }
}