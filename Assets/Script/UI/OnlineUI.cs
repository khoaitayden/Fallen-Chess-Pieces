using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;
using Mirror.Discovery;

public class OnlineUI : MonoBehaviour
{
    public static OnlineUI Instance { get; private set; }

    [Header("Panels")]
    [SerializeField] private GameObject mainOnlinePanel;
    [SerializeField] private GameObject createRoomPanel;
    [SerializeField] private GameObject joinRoomPanel;
    [SerializeField] private PasswordPopupUI passwordPopup;

    // --- MISSING VARIABLES ADDED BACK ---
    [Header("Main Panel Buttons")]
    [SerializeField] private Button mainCreateRoomButton;
    [SerializeField] private Button mainJoinRoomButton;
    [SerializeField] private Button mainBackButton;
    // ------------------------------------

    [Header("Create Room Panel")]
    [SerializeField] private TMP_InputField createRoomNameInput;
    [SerializeField] private TMP_InputField createPasswordInput;
    [SerializeField] private Toggle hostLocalToggle;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button cancelCreateButton;

    [Header("Join Room Panel")]
    [SerializeField] private TMP_InputField joinByAddressInput;
    [SerializeField] private Button joinByAddressButton;
    [SerializeField] private Button cancelJoinButton;

    [Header("Server List")]
    [SerializeField] private GameObject serverListContent;
    [SerializeField] private GameObject serverListItemPrefab;

    private CustomNetworkDiscovery _networkDiscovery;
    private string _passwordForJoinAttempt = "";

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _networkDiscovery = NetworkManager.singleton.GetComponent<CustomNetworkDiscovery>();
        SetupButtonListeners();
    }

    // Re-added OnEnable/OnDisable to correctly handle discovery events.
    private void OnEnable()
    {
        if (_networkDiscovery != null)
        {
            _networkDiscovery.OnServerFound.AddListener(OnServerFound);
        }
    }

    private void OnDisable()
    {
        if (_networkDiscovery != null)
        {
            _networkDiscovery.OnServerFound.RemoveListener(OnServerFound);
        }
    }

    private void SetupButtonListeners()
    {
        // These calls are now valid because the variables are declared above.
        mainCreateRoomButton.onClick.AddListener(() => ShowPanel(createRoomPanel));
        mainJoinRoomButton.onClick.AddListener(OnJoinRoomMenuClicked);
        mainBackButton.onClick.AddListener(OnBackButtonClicked);

        hostButton.onClick.AddListener(OnHostButtonClicked);
        cancelCreateButton.onClick.AddListener(() => ShowPanel(mainOnlinePanel));
        joinByAddressButton.onClick.AddListener(OnJoinByAddressClicked);
        cancelJoinButton.onClick.AddListener(() => {
            _networkDiscovery?.StopDiscovery();
            ShowPanel(mainOnlinePanel);
        });
    }

    private void OnHostButtonClicked()
    {
        CustomNetworkManager cnm = NetworkManager.singleton as CustomNetworkManager;
        if (cnm == null) return;
        cnm.RoomName = createRoomNameInput.text;
        cnm.RoomPassword = createPasswordInput.text;
        NetworkManager.singleton.StartHost();
        if (hostLocalToggle.isOn)
        {
            _networkDiscovery?.AdvertiseServer();
        }
        UIManager.Instance.ShowLobbyPanel();
    }

    private void OnJoinRoomMenuClicked()
    {
        ShowPanel(joinRoomPanel);
        if (_networkDiscovery != null)
        {
            _networkDiscovery.discoveredServers.Clear();
            UpdateServerList();
            _networkDiscovery.StartDiscovery();
        }
    }

    private void OnJoinByAddressClicked()
    {
        string address = joinByAddressInput.text;
        if (string.IsNullOrEmpty(address)) return;
        passwordPopup.Show((password) => {
            AttemptToJoinServer(address, password);
        });
    }

    private void OnBackButtonClicked()
    {
        _networkDiscovery?.StopDiscovery();
        UIManager.Instance.ShowMenuPanel();
    }

    public void JoinServer(DiscoveryResponse serverInfo)
    {
        _networkDiscovery?.StopDiscovery();
        if (serverInfo.hasPassword)
        {
            passwordPopup.Show((password) => {
                AttemptToJoinServer(serverInfo.uri.Host, password);
            });
        }
        else
        {
            AttemptToJoinServer(serverInfo.uri.Host, "");
        }
    }

    private void AttemptToJoinServer(string address, string password)
    {
        _passwordForJoinAttempt = password;
        NetworkManager.singleton.networkAddress = address;
        NetworkManager.singleton.StartClient();
        UIManager.Instance.ShowLobbyPanel();
    }

    private void OnServerFound(DiscoveryResponse info)
    {
        _networkDiscovery.discoveredServers[info.serverId] = info;
        UpdateServerList();
    }

    public void UpdateServerList()
    {
        foreach (Transform child in serverListContent.transform) Destroy(child.gameObject);
        if (_networkDiscovery == null) return;
        foreach (DiscoveryResponse serverInfo in _networkDiscovery.discoveredServers.Values)
        {
            GameObject listItem = Instantiate(serverListItemPrefab, serverListContent.transform);
            listItem.GetComponent<ServerListItem>().Setup(serverInfo);
        }
    }

    private void ShowPanel(GameObject panelToShow)
    {
        mainOnlinePanel.SetActive(panelToShow == mainOnlinePanel);
        createRoomPanel.SetActive(panelToShow == createRoomPanel);
        joinRoomPanel.SetActive(panelToShow == joinRoomPanel);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        ShowPanel(mainOnlinePanel);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public string GetPasswordForJoin()
    {
        return _passwordForJoinAttempt;
    }
}