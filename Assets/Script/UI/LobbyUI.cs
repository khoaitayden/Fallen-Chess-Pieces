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

    private void OnEnable()
    {
        startGameButton.gameObject.SetActive(false);
        statusText.text = "Connecting to lobby...";
        roomCodeText.text = "";

        if (NetworkServer.active && NetworkClient.isConnected)
        {
            UpdateLobbyUI(1, true);
        }
    }

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);

    public void UpdateLobbyUI(int playerCount, bool amIHost)
    {
        startGameButton.gameObject.SetActive(amIHost && playerCount == 2);
        statusText.text = (playerCount < 2) ? "Waiting for opponent..." : "Opponent has joined!";

        if (amIHost)
        {
            string ipAddress = NetworkUtils.GetLocalIPv4();
            string roomName = (NetworkManager.singleton as CustomNetworkManager)?.RoomName ?? "Chess Game";
            roomCodeText.text = $"IP: {ipAddress} | Room: {roomName}";
        }
        else
        {
            roomCodeText.text = "Connected to host";
        }
    }

    private void OnStartGameClicked()
    {
        (NetworkManager.singleton as CustomNetworkManager)?.ServerStartGame();
    }

    private void OnLeaveLobbyClicked()
    {
        if (NetworkClient.localPlayer != null)
        {
            NetworkPlayer localPlayer = NetworkClient.localPlayer.GetComponent<NetworkPlayer>();
            if (localPlayer != null)
            {
                localPlayer.CmdLeaveLobby();
            }
        }
        else
        {
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                NetworkManager.singleton.StopHost();
            }
            else if (NetworkClient.isConnected)
            {
                NetworkManager.singleton.StopClient();
            }
        }
    }
}