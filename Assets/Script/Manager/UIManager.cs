using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Child UI Controllers")]
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

    public void ShowPromotionPanel(bool isWhite)
    {
        promotionUI.ShowPanel(isWhite);
    }

    public void HidePromotionPanel()
    {
        promotionUI.HidePanel();
    }
    public void ShowGameplayPanel()
    {
        gameplayUI.ShowPanel();
    }
    public void HideGameplayPanel()
    {
        gameplayUI.HidePanel();
    }
}