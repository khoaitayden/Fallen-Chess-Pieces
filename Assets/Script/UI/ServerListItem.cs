using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Mirror;

public class ServerListItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI serverNameText;

    private DiscoveryResponse _serverInfo;

    public void Setup(DiscoveryResponse serverInfo)
    {
        _serverInfo = serverInfo;

        serverNameText.text = serverInfo.roomName;
        
        GetComponent<Button>().onClick.AddListener(OnListItemClicked);
    }

    private void OnListItemClicked()
    {
        OnlineUI.Instance.JoinServer(_serverInfo);
    }
}