using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RestartButtonHandler : MonoBehaviour
{
    [SerializeField] private Button myButton;

    void Start()
    {
        // Make sure it's cleared if you want only this listener
        myButton.onClick.RemoveAllListeners();

        // Add a new listener
        myButton.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        GameLogic.Instance.RestartLevel();
    }
}