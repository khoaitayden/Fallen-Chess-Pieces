using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
using System.Collections;

public class LobbyUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI roomCodeText;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button leaveLobbyButton;

    private string _roomCode;
    private Coroutine _updateRoomInfoCoroutine;

    private void Awake()
    {
        startGameButton.onClick.AddListener(OnStartGameClicked);
        leaveLobbyButton.onClick.AddListener(OnLeaveLobbyClicked);
    }

    private void OnEnable()
    {
        startGameButton.gameObject.SetActive(false);
        if (_updateRoomInfoCoroutine != null) StopCoroutine(_updateRoomInfoCoroutine);
        _updateRoomInfoCoroutine = StartCoroutine(UpdateRoomInfoRoutine());
    }

    private void OnDisable()
    {
        if (_updateRoomInfoCoroutine != null)
        {
            StopCoroutine(_updateRoomInfoCoroutine);
            _updateRoomInfoCoroutine = null;
        }
    }

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);

    public void UpdateLobbyUI(int playerCount, bool amIHost)
    {
        startGameButton.gameObject.SetActive(amIHost && playerCount == 2);
        statusText.text = (playerCount < 2) ? "Waiting for opponent..." : "Opponent has joined!";
    }

    private IEnumerator UpdateRoomInfoRoutine()
    {
        if (!NetworkServer.active)
        {
            roomCodeText.text = "Connected to host";
            yield break;
        }

        roomCodeText.text = "Generating room info...";
        string ipAddress = "resolving...";

        while (ipAddress == "resolving..." || ipAddress == "localhost" || ipAddress == "127.0.0.1")
        {
            ipAddress = NetworkUtils.GetLocalIPv4();
            yield return new WaitForSeconds(0.5f);
        }

        if (string.IsNullOrEmpty(_roomCode))
        {
            _roomCode = NetworkUtils.GenerateRoomCode(5);
        }
        roomCodeText.text = $"IP: {ipAddress}  |  Code: {_roomCode}";
    }

    private void OnStartGameClicked()
    {
        (NetworkManager.singleton as CustomNetworkManager)?.ServerStartGame();
    }

    private void OnLeaveLobbyClicked()
    {
        _roomCode = "";
        
        (NetworkManager.singleton as CustomNetworkManager)?.Shutdown();
        
        UIManager.Instance.ShowMenuPanel();
    }
}