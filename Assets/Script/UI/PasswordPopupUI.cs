using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
using System;

public class PasswordPopupUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private Button confirmJoinButton;
    [SerializeField] private Button cancelJoinButton;

    private Action<string> onConfirm;

    private void Awake()
    {
        confirmJoinButton.onClick.AddListener(OnConfirmClicked);
        cancelJoinButton.onClick.AddListener(OnCancelClicked);
    }

    public void Show(Action<string> onConfirmAction)
    {
        this.onConfirm = onConfirmAction;
        passwordInput.text = ""; // Clear previous password
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnConfirmClicked()
    {
        onConfirm?.Invoke(passwordInput.text);
        Hide();
    }

    private void OnCancelClicked()
    {
        Hide();
    }
}