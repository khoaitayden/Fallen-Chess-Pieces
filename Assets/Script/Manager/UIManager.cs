using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Controllers")]
    [SerializeField] private MainMenuUI menuUI;
    [SerializeField] private ChooseAIDifficultyUI chooseAIDifficultyUI; // Add reference to the new UI
    [SerializeField] private GameplayUI gameplayUI;
    [SerializeField] private GameOverUI gameOverUI;
    [SerializeField] private PromotionUI promotionUI;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        // When the game first starts, show the main menu.
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
        // When the game ends, show the game over screen on top of the gameplay screen.
        if (newState == GameState.Checkmate || newState == GameState.Stalemate || newState == GameState.Timeout || newState == GameState.Draw)
        {
            gameOverUI.Show(newState);
        }
    }

    // --- PANEL MANAGEMENT METHODS ---
    public void ShowMenuPanel()
    {
        menuUI.Show();
        chooseAIDifficultyUI.Hide(); // Make sure other panels are hidden
        gameplayUI.gameObject.SetActive(false); // Hide the gameplay UI
    }

    public void ShowAIDifficultyPanel()
    {
        menuUI.Hide();
        chooseAIDifficultyUI.Show(); // Show the difficulty panel
        gameplayUI.gameObject.SetActive(false);
    }

    public void ShowGameplayPanel()
    {
        menuUI.Hide();
        chooseAIDifficultyUI.Hide();
        gameplayUI.gameObject.SetActive(true); // Show the gameplay UI
        // Also call the ShowPanel method if it exists
        gameplayUI.ShowPanel();
    }

    public void HideGameplayPanel()
    {
        gameplayUI.HidePanel();
    }

    public void ShowPromotionPanel(bool isWhite)
    {
        promotionUI.ShowPanel(isWhite);
    }

    public void HidePromotionPanel()
    {
        promotionUI.HidePanel();
    }

    public void HideAIDifficultyPanel()
    {
        chooseAIDifficultyUI.Hide();
    }
}