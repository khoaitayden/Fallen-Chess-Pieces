using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Controllers")]
    [SerializeField] private MainMenuUI menuUI;
    [SerializeField] private OnlineUI onlineUI;
    [SerializeField] private LobbyUI lobbyUI;
    [SerializeField] private ChooseAIDifficultyUI chooseAIDifficultyUI;
    [SerializeField] private GameplayUI gameplayUI;
    [SerializeField] private GameOverUI gameOverUI;
    [SerializeField] private PromotionUI promotionUI;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Make the UIManager persistent
    }

    private void Start()
    {
        ShowMenuPanel();
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
        }
    }

    private void HandleGameStateChanged(GameState newState)
    {
        if (newState == GameState.Checkmate || newState == GameState.Stalemate || newState == GameState.Timeout || newState == GameState.Draw)
        {
            gameOverUI.Show(newState);
        }
    }

    // --- ROBUST PANEL MANAGEMENT ---
    private void HideAllPanels()
    {
        menuUI.Hide();
        onlineUI.Hide();
        lobbyUI.Hide();
        chooseAIDifficultyUI.Hide();
        gameplayUI.HidePanel();
    }

    public void ShowMenuPanel()
    {
        HideAllPanels();
        menuUI.Show();
    }

    public void ShowOnlinePanel()
    {
        HideAllPanels();
        onlineUI.Show();
    }

    public void ShowLobbyPanel()
    {
        HideAllPanels();
        lobbyUI.Show();
    }

    public void ShowAIDifficultyPanel()
    {
        HideAllPanels();
        chooseAIDifficultyUI.Show();
    }

    public void ShowGameplayPanel()
    {
        HideAllPanels();
        gameplayUI.ShowPanel();
    }

    public void ShowPromotionPanel(bool isWhite) => promotionUI.ShowPanel(isWhite);
    public void HidePromotionPanel() => promotionUI.HidePanel();
}