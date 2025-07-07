using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Controllers")]
    [SerializeField] private MainMenuUI menuUI;
    [SerializeField] private ChooseAIDifficultyUI chooseAIDifficultyUI;
    [SerializeField] private GameplayUI gameplayUI;
    [SerializeField] private GameOverUI gameOverUI;
    [SerializeField] private PromotionUI promotionUI;
    [SerializeField] private OnlineUI onlineUI;

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
        chooseAIDifficultyUI.Hide();
        gameplayUI.gameObject.SetActive(false); 
    }

    public void ShowAIDifficultyPanel()
    {
        menuUI.Hide();
        chooseAIDifficultyUI.Show(); 
        gameplayUI.gameObject.SetActive(false);
    }

    public void ShowGameplayPanel()
    {
        menuUI.Hide();
        chooseAIDifficultyUI.Hide();
        gameplayUI.gameObject.SetActive(true); 
        gameplayUI.ShowPanel();
    }
    public void ShowOnlinePanel()
    {
        menuUI.Hide();
        onlineUI.Show();
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