using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class LobbyUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI roomCodeText;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button leaveLobbyButton;

    private void Awake()
    {
        startGameButton.onClick.AddListener(OnStartGameClicked);
        leaveLobbyButton.onClick.AddListener(OnLeaveLobbyClicked);
    }

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);

    public void UpdateLobbyUI(int playerCount, bool isHost)
    {
        // The Start Game button should only be clickable by the host, and only when there are 2 players.
        startGameButton.gameObject.SetActive(isHost && playerCount == 2);

        if (playerCount < 2)
        {
            statusText.text = "Waiting for opponent...";
        }
        else
        {
            statusText.text = "Opponent has joined!";
        }

        if (isHost)
        {
            roomCodeText.text = $"Room Code: {NetworkManager.singleton.networkAddress}";
        }
        else
        {
            roomCodeText.text = "";
        }
    }

    private void OnStartGameClicked()
    {
        (NetworkManager.singleton as CustomNetworkManager)?.ServerStartGame();
    }

    private void OnLeaveLobbyClicked()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else if (NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopClient();
        }
        UIManager.Instance.ShowMenuPanel();
    }
}