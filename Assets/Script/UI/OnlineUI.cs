// In OnlineUI.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
using Mirror.Discovery; // Required for ServerResponse
using System.Collections.Generic;

public class OnlineUI : MonoBehaviour
{
    public static OnlineUI Instance { get; private set; }

    [Header("Main Panel")]
    [SerializeField] private Toggle hostLocalToggle;
    [SerializeField] private Button createRoomButton;
    [SerializeField] private Button joinRoomButton;
    [SerializeField] private Button backButton;

    [Header("Join Room Panel")]
    [SerializeField] private GameObject joinRoomPanel;
    [SerializeField] private TMP_InputField addressInput;
    [SerializeField] private Button joinByAddressButton;
    [SerializeField] private Button cancelJoinButton;

    [Header("Server List")]
    [SerializeField] private GameObject serverListContent;
    [SerializeField] private GameObject serverListItemPrefab;

    private LanDiscoveryHelper _lanHelper;

    private void Awake()
    {
        Instance = this;
        _lanHelper = NetworkManager.singleton.GetComponent<LanDiscoveryHelper>();

        createRoomButton.onClick.AddListener(OnCreateRoomClicked);
        joinRoomButton.onClick.AddListener(OnJoinRoomClicked);
        backButton.onClick.AddListener(OnBackButtonClicked);
        joinByAddressButton.onClick.AddListener(OnJoinByAddressClicked);
        cancelJoinButton.onClick.AddListener(OnCancelJoinClicked);
    }

    private void OnCreateRoomClicked()
    {
        if (hostLocalToggle.isOn)
        {
            // Use the helper to start the host AND advertise.
            _lanHelper?.StartHostAndAdvertise();
        }
        else
        {
            // If not hosting locally, just start the host without advertising.
            NetworkManager.singleton.StartHost();
        }
        UIManager.Instance.ShowLobbyPanel();
    }

    private void OnJoinRoomClicked()
    {
        joinRoomPanel.SetActive(true);
        _lanHelper?.StartSearching();
    }

    private void OnJoinByAddressClicked()
    {
        NetworkManager.singleton.networkAddress = addressInput.text;
        NetworkManager.singleton.StartClient();
        Hide();
        UIManager.Instance.ShowLobbyPanel();
    }

    private void OnCancelJoinClicked()
    {
        CloseJoinRoomPanel();
    }

    private void OnBackButtonClicked()
    {
        _lanHelper?.Stop();
        UIManager.Instance.ShowMenuPanel();
    }

    public void UpdateServerList()
    {
        foreach (Transform child in serverListContent.transform)
        {
            Destroy(child.gameObject);
        }

        if (_lanHelper == null) return;

        // Use the dictionary from the helper script.
        foreach (ServerResponse serverInfo in _lanHelper.discoveredServers.Values)
        {
            GameObject listItem = Instantiate(serverListItemPrefab, serverListContent.transform);
            // Pass the ServerResponse object to the list item.
            listItem.GetComponent<ServerListItem>().Setup(serverInfo);
        }
    }

    public void CloseJoinRoomPanel()
    {
        _lanHelper?.Stop();
        joinRoomPanel.SetActive(false);
    }

    public void Show()
    {
        joinRoomPanel.SetActive(false);
        gameObject.SetActive(true);
    }

    public void Hide()
    {

        gameObject.SetActive(false);
    }
}