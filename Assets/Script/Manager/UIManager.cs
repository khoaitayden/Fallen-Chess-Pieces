using UnityEngine;
using UnityEngine.UI; // Required for UI elements like Text
using TMPro; // Required for TextMeshPro elements

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Panels")]
    [SerializeField] private GameObject gameplayPanel;
    [SerializeField] private GameObject gameOverPanel;

    [Header("Game Over UI")]
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private Button rematchButton;
    [SerializeField] private Button quitButton;

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

        rematchButton.onClick.AddListener(OnRematchClicked);
        quitButton.onClick.AddListener(OnQuitClicked);
    }

    private void Start()
    {
        ShowPanel(gameplayPanel);
        
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
        if (newState == GameState.Checkmate || newState == GameState.Stalemate || newState == GameState.Timeout)
        {
            ShowGameOver(newState);
        }
    }

    private void ShowPanel(GameObject panelToShow)
    {
        gameplayPanel.SetActive(panelToShow == gameplayPanel);
        gameOverPanel.SetActive(panelToShow == gameOverPanel);
        // menuPanel.SetActive(panelToShow == menuPanel);
    }

    public void ShowGameOver(GameState finalState)
    {
        ShowPanel(gameOverPanel);

        if (finalState == GameState.Checkmate)
        {
            bool winnerIsWhite = !TurnManager.Instance.IsWhiteTurn;
            winnerText.text = $"Checkmate!\n{(winnerIsWhite ? "White" : "Black")} Wins!";
            
            winnerText.color = winnerIsWhite ? Color.white : Color.black;
            winnerText.outlineWidth = 0.3f;
            winnerText.outlineColor = winnerIsWhite ? Color.black : Color.white;
        }
        else if (finalState == GameState.Stalemate)
        {
            winnerText.text = "Stalemate!\nIt's a Draw!";
            
            winnerText.color = Color.green;
            winnerText.outlineWidth = 0f;
        } else if (finalState == GameState.Timeout)
        {
            bool winnerIsWhite = TurnManager.Instance.BlackTime <= 0; 
            winnerText.text = $"Time Out!\n{(winnerIsWhite ? "White" : "Black")} Wins!";
            
            winnerText.color = winnerIsWhite ? Color.white : Color.black;
            winnerText.outlineWidth = 0.1f;
            winnerText.outlineColor = winnerIsWhite ? Color.black : Color.white;
        }
    }

    private void OnRematchClicked()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    private void OnQuitClicked()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}