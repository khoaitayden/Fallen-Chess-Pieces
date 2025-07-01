using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; // Required for reloading the scene

public class GameOverUI : MonoBehaviour
{
    public static GameOverUI Instance { get; private set; }

    [Header("UI Elements")]
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

    public void Show(GameState finalState)
    {
        gameObject.SetActive(true);

        switch (finalState)
        {
            case GameState.Checkmate:
                bool winnerIsWhite = !TurnManager.Instance.IsWhiteTurn;
                winnerText.text = $"Checkmate!\n{(winnerIsWhite ? "White" : "Black")} Wins!";
                winnerText.color = winnerIsWhite ? Color.white : Color.black;
                winnerText.outlineColor = winnerIsWhite ? Color.black : Color.white;
                winnerText.outlineWidth = 0.1f;
                break;

            case GameState.Timeout:
                bool timedOutWinnerIsWhite = TurnManager.Instance.BlackTime <= 0;
                winnerText.text = $"Time Out!\n{(timedOutWinnerIsWhite ? "White" : "Black")} Wins!";
                winnerText.color = timedOutWinnerIsWhite ? Color.white : Color.black;
                winnerText.outlineColor = timedOutWinnerIsWhite ? Color.black : Color.white;
                winnerText.outlineWidth = 0.1f;
                break;

            case GameState.Stalemate:
                winnerText.text = "Stalemate!\nIt's a Draw!";
                winnerText.color = Color.gray;
                winnerText.outlineWidth = 0f;
                break;

            case GameState.Draw:
                winnerText.text = "Draw";
                winnerText.color = Color.gray;
                winnerText.outlineWidth = 0f;
                break;
        }
    }

    private void OnRematchClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnQuitClicked()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}