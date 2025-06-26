using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    public bool IsWhiteTurn { get; private set; }

    public float WhiteTime { get; private set; }
    public float BlackTime { get; private set; }
    private bool isTimerActive = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    void Start()
    {
        StartNewGame();
    }

    public void StartNewGame()
    {
        IsWhiteTurn = true;
        WhiteTime = Constants.DEFAULT_GAME_TIME;
        BlackTime = Constants.DEFAULT_GAME_TIME;
        isTimerActive = true;
    }

    void Update()
    {
        if (!isTimerActive) return;

        if (IsWhiteTurn)
        {
            WhiteTime -= Time.deltaTime;
            if (WhiteTime <= 0)
            {
                WhiteTime = 0;
                HandleTimeout();
            }
        }
        else
        {
            BlackTime -= Time.deltaTime;
            if (BlackTime <= 0)
            {
                BlackTime = 0;
                HandleTimeout();
            }
        }
    }

    public void SwitchTurn()
    {
        IsWhiteTurn = !IsWhiteTurn;
    }

    private void HandleTimeout()
    {
        isTimerActive = false;
        bool winnerIsWhite = IsWhiteTurn; 
        GameManager.Instance.EndGame(GameState.Timeout, winnerIsWhite);
    }

    public void StopTimer()
    {
        isTimerActive = false;
    }
}