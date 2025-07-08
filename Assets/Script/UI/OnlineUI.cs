// In OnlineUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class OnlineUI : MonoBehaviour
{
    [Header("Main Panel")]
    [SerializeField] private Button createRoomButton;
    [SerializeField] private Button joinRoomButton;
    [SerializeField] private Button matchmakingButton;
    [SerializeField] private Button backButton;

    [Header("Join Room Panel")]
    [SerializeField] private GameObject joinRoomPanel;
    [SerializeField] private TMP_InputField addressInput;
    [SerializeField] private Button joinButton;
    [SerializeField] private Button cancelJoinButton;

    private void Awake()
    {
        createRoomButton.onClick.AddListener(OnCreateRoomClicked);
        joinRoomButton.onClick.AddListener(OnJoinRoomClicked);
        matchmakingButton.onClick.AddListener(() => Debug.Log("Matchmaking not implemented."));
        backButton.onClick.AddListener(() => UIManager.Instance.ShowMenuPanel());

        joinButton.onClick.AddListener(OnJoinButtonClicked);
        cancelJoinButton.onClick.AddListener(OnCancelJoinClicked);
    }

    private void OnCreateRoomClicked()
    {
        NetworkManager.singleton.StartHost();
        UIManager.Instance.ShowLobbyPanel();
    }

    private void OnJoinRoomClicked()
    {
        joinRoomPanel.SetActive(true);
    }

    private void OnJoinButtonClicked()
    {
        string address = addressInput.text;
        if (string.IsNullOrEmpty(address))
        {
            address = "localhost";
        }
        
        NetworkManager.singleton.networkAddress = address;
        NetworkManager.singleton.StartClient();
        UIManager.Instance.ShowLobbyPanel();
        Hide();
    }

    private void OnCancelJoinClicked()
    {
        joinRoomPanel.SetActive(false);
    }
    
    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
}