using DraasGames.Core.Runtime.UI.Extensions;
using DraasGames.Core.Runtime.UI.PresenterNavigationService.Abstract;
using UnityEngine;
using UnityEngine.UI;

public class TestButtonPress : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private CustomButton _customButton;

    private void Start()
    {
        _button.onClick.AddListener(OnButtonClick);
        _customButton.OnClick += (OnButtonClick);
    }

    private void OnButtonClick()
    {
        Debug.Log("Button clicked");
    }
}
