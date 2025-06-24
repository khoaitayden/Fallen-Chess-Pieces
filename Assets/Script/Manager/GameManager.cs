using UnityEngine;



public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState CurrentState { get; private set; }

    //Notify when change state
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
            OnGameStateChanged?.Invoke(finalState); 

            if (finalState == GameState.Checkmate)
            {
                Debug.Log($"GAME OVER: CHECKMATE! Winner: {(winnerIsWhite ? "White" : "Black")}");
            }
            else if (finalState == GameState.Stalemate)
            {
                Debug.Log("GAME OVER: STALEMATE! It's a draw.");
            }
        }
    }
}