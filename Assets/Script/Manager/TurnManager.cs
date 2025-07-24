using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }
    public Vector2Int EnPassantTargetSquare { get; private set; }
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
        
        // --- THIS IS THE FIX ---
        // Instead of a constant, get the game time from the current game's settings.
        WhiteTime = GameManager.Instance.CurrentSettings.GameTime;
        BlackTime = GameManager.Instance.CurrentSettings.GameTime;
        // -----------------------

        isTimerActive = true;
        EnPassantTargetSquare = new Vector2Int(-1, -1);
    }

    void Update()
    {
        if (!isTimerActive || GameManager.Instance.CurrentState == GameState.PowerTransfer) return;

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
    public void SetEnPassantTarget(ChessPiece piece, Vector2Int from, Vector2Int to)
    {
        if (piece.Type == PieceType.Pawn && Mathf.Abs(to.y - from.y) == 2)
        {
            int direction = (piece.IsWhite) ? -1 : 1;
            EnPassantTargetSquare = new Vector2Int(to.x, to.y + direction);
        }
        else
        {
            EnPassantTargetSquare = new Vector2Int(-1, -1);
        }
    }
    public void SwitchTurn()
    {
        // --- NEW: TIME INCREMENT LOGIC ---
        // Get the time increment value from the settings.
        float increment = GameManager.Instance.CurrentSettings.TimeIncrement;
        if (increment > 0)
        {
            // Add the increment to the player who just finished their turn.
            if (IsWhiteTurn)
            {
                WhiteTime += increment;
            }
            else
            {
                BlackTime += increment;
            }
        }
        // ---------------------------------

        IsWhiteTurn = !IsWhiteTurn;
        if (GameManager.Instance.CurrentGameMode == GameMode.Local)
        {
            BoardPresenter.Instance.OrientBoardToPlayer(IsWhiteTurn);
        }
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