using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DraasGames.Core.Runtime.UI.SlideCarousel
{
    [RequireComponent(typeof(ScrollRect))]
    public class Carousel : MonoBehaviour, IBeginDragHandler, IEndDragHandler
    {
        public event Action<int> OnActivityChanged;
        
        [SerializeField] private bool _controlWithDrag = true;
        
        //todo для всех частных случаях можно заменить на конкретные енами в наследниках и их уже приводить к int
        [SerializeField] protected int defaultActivityIndex = 0;
        [Range(.05f, .5f)]
        [SerializeField] private double dragSensitivityMultiplier = .1f;

        [SerializeField] private float lerpDuration = .1f;
        
        private ScrollRect _scrollRect;

        private ISetOptionState[] _options;
        private ISetOptionState _currentOption;
        private ISetOptionState _lastOption;
        
        protected int _currentOptionIndex;

        private Vector2 _beginScrollPosition;
        private Vector2 _lastScrollPosition;
        
        private float _threshold = 0;

        private float _startDragPosition;
        private float _endDragPosition;
        private float _delta;
        private double _dragSensitivity;

        private void Start()
        {
            Initialize();
            
            _scrollRect.onValueChanged.AddListener(UpdateScrollPosition);
        }
        
        public void Initialize()
        {
            _dragSensitivity = Screen.width * dragSensitivityMultiplier;
            _scrollRect = GetComponent<ScrollRect>();
            _options = GetComponentsInChildren<ISetOptionState>();

            if (_options.Length <= 1)
            {
                _threshold = 1;
            }
            else
            {
                _threshold = 1f / (_options.Length - 1f);
            }

            _scrollRect.horizontalNormalizedPosition = 1f;

            SetDefaultActivityOption();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if(!_controlWithDrag) 
                return;
            
            _startDragPosition = eventData.position.x;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if(!_controlWithDrag) 
                return;
            
            _endDragPosition = eventData.position.x;
                                                         
            _delta = _endDragPosition - _startDragPosition;
            
            //Свайп влево
            if (_delta < _dragSensitivity * -1)
            {
                if (_currentOptionIndex + 1 > _options.Length - 1) return;
                
                ChangeActivity(_currentOptionIndex + 1);
            }
            //свайп вправо
            else if (_delta > _dragSensitivity)
            {
                if (_currentOptionIndex == 0) return;

                ChangeActivity(_currentOptionIndex - 1);
            }
            else
            {
                ChangeActivity(_currentOptionIndex); 
            }
        }

        public int GetItemsAmount()
        {
            return _options.Length;
        }
        
        public int GetCurrentOptionIndex()
        {
            return _currentOptionIndex;
        }
        
        public GameObject GetCurrentGameObject()
        {
            return _currentOption.GameObject;
        }
        
        private void UpdateScrollPosition(Vector2 position)
        {
            _lastScrollPosition = position;

            if (_lastScrollPosition.x > 1) _lastScrollPosition.x = 1;
        }
        
        private IEnumerator LerpScrollValue(float targetValue, float duration = 1f)
        {
            float t = 0f;
            float initialValue = _scrollRect.horizontalNormalizedPosition;
            
            while (t < duration)
            {
                _scrollRect.horizontalNormalizedPosition = Mathf.Lerp(initialValue, targetValue, t / duration);
                t += Time.deltaTime;
                yield return null;
            }

            _scrollRect.horizontalNormalizedPosition = targetValue;
        }

        private void SetDefaultActivityOption()
        {
            SetActivity(defaultActivityIndex, true);
        }

        private void SetScrollActivityPosition(float position, bool instant = false)
        {
            if (instant)
            {
                _scrollRect.horizontalNormalizedPosition = position;
            }
            else
            {
                StartCoroutine(LerpScrollValue(position, lerpDuration));
            }
        }

        public void ChangeActivity(int newActivityIndex, bool instant = false)
        {
            _lastOption = _currentOption;
            _currentOption = _options[newActivityIndex];

            _lastOption.SetOptionState(false);
            _currentOption.SetOptionState(true);

            _currentOptionIndex = newActivityIndex;
            OnActivityChanged?.Invoke(newActivityIndex);
            
            SetScrollActivityPosition(newActivityIndex * _threshold, instant);
            //Debug.Log($"New normalized position is {newActivityIndex * _threshold}");
        }

        protected void SetActivity(int newActivityIndex, bool instant = false)
        {
            if (newActivityIndex > _options.Length - 1) return;

            _currentOption = _options[newActivityIndex];
            _currentOptionIndex = newActivityIndex;

            _currentOption.SetOptionState(true);
            OnActivityChanged?.Invoke(newActivityIndex);
            
            SetScrollActivityPosition(newActivityIndex * _threshold, instant);
        }
    }
}