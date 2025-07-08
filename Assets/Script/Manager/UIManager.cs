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
        if (Instance != null && Instance != this) { Destroy(gameObject); }
        else { Instance = this; }
    }

    private void Start()
    {
        // When the game first launches, only the main menu should be visible.
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


    public void ShowMenuPanel()
    {
        menuUI.Show();
        onlineUI.Hide();
        lobbyUI.Hide();
        chooseAIDifficultyUI.Hide();
        gameplayUI.HidePanel();
    }

    public void ShowOnlinePanel()
    {
        menuUI.Hide();
        onlineUI.Show();
        lobbyUI.Hide();
    }

    public void ShowLobbyPanel()
    {
        menuUI.Hide();
        onlineUI.Hide();
        lobbyUI.Show();
    }

    public void ShowAIDifficultyPanel()
    {
        menuUI.Hide();
        onlineUI.Hide();
        chooseAIDifficultyUI.Show();
    }

    public void ShowGameplayPanel()
    {
        menuUI.Hide();
        onlineUI.Hide();
        lobbyUI.Hide();
        chooseAIDifficultyUI.Hide();
        
        gameplayUI.ShowPanel();
    }


    public void ShowPromotionPanel(bool isWhite)
    {
        promotionUI.ShowPanel(isWhite);
    }

    public void HidePromotionPanel()
    {
        promotionUI.HidePanel();
    }
}