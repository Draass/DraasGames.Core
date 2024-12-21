using DraasGames.Core.Runtime.UI.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class TestButtonPress : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private CustomButton _customButton;
    
    private void Start()
    {
        _button.onClick.AddListener(OnButtonClick);
        _customButton.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        Debug.Log("Button clicked");
    }
}
