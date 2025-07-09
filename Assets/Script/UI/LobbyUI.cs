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
        startGameButton.gameObject.SetActive(isHost && playerCount == 2);

        statusText.text = (playerCount < 2) ? "Waiting for opponent..." : "Opponent has joined!";

        if (isHost)
        {
            // Use our NetworkUtils helper to show the real, connectable IP address.
            roomCodeText.text = $"Room IP: {NetworkUtils.GetLocalIPv4()}";
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