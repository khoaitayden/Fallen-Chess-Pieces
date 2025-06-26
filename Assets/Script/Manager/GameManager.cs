using UnityEngine;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState CurrentState { get; private set; }

    public event System.Action<GameState> OnGameStateChanged;

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
        StartNewGame();
    }

    public void StartNewGame()
    {
        CurrentState = GameState.Playing;
        Debug.Log("New game started. It's White's turn.");
    }

    public void EndGame(GameState finalState, bool winnerIsWhite)
    {
        if (CurrentState == GameState.Playing)
        {
            CurrentState = finalState;
            TurnManager.Instance.StopTimer();
            OnGameStateChanged?.Invoke(finalState);
        }
    }
    public void CheckForGameEnd()
    {
        bool currentPlayerIsWhite = TurnManager.Instance.IsWhiteTurn;

        if (MoveValidator.Instance.IsCheckmate(currentPlayerIsWhite))
        {
            bool winnerIsWhite = !currentPlayerIsWhite;
            EndGame(GameState.Checkmate, winnerIsWhite);
        }
        else if (MoveValidator.Instance.IsStalemate(currentPlayerIsWhite))
        {

            EndGame(GameState.Stalemate, false);
        }
    }
}