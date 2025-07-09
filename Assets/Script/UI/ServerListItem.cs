// In ServerListItem.cs
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Mirror;
using Mirror.Discovery; // Required for ServerResponse

public class ServerListItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI serverNameText;
    private ServerResponse _serverInfo;

    public void Setup(ServerResponse serverInfo)
    {
        _serverInfo = serverInfo;
        // Display the server's actual address, which is more useful.
        serverNameText.text = $"Game at: {_serverInfo.uri.Host}";
        GetComponent<Button>().onClick.AddListener(OnJoinClicked);
    }

    private void OnJoinClicked()
    {
        // Stop searching for other servers once we've decided to join one.
        NetworkManager.singleton.GetComponent<LanDiscoveryHelper>().Stop();

        NetworkManager.singleton.networkAddress = _serverInfo.uri.Host;
        NetworkManager.singleton.StartClient();

        OnlineUI.Instance.CloseJoinRoomPanel();
        UIManager.Instance.ShowLobbyPanel();
    }
}