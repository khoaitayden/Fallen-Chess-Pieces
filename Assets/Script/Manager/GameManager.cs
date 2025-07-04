using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameMode CurrentGameMode { get; private set; }
    public GameState CurrentState { get; private set; }

    // --- PLAYER MANAGEMENT ---
    private Player whitePlayer;
    private Player blackPlayer;
    // -------------------------

    public event System.Action<GameState> OnGameStateChanged;
    private ChessPiece _pawnToPromote;

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
        // The GameManager no longer starts a game by default.
        // It waits for a button press from the MenuUI.
    }

    public void StartNewGame(GameMode mode)
    {
        CurrentGameMode = mode;

        // --- CREATE PLAYERS BASED ON MODE ---
        switch (CurrentGameMode)
        {
            case GameMode.Local:
                whitePlayer = new HumanPlayer(true);
                blackPlayer = new HumanPlayer(false);
                BoardPresenter.Instance.OrientBoardToPlayer(true); 
                break;
            case GameMode.AI:
                whitePlayer = new HumanPlayer(true);
                blackPlayer = new AIPlayer(false); // Example: Player is White, AI is Black
                break;
            case GameMode.Online:
                // This would be handled by your networking logic later
                // For now, default to local players
                whitePlayer = new HumanPlayer(true);
                blackPlayer = new HumanPlayer(false);
                break;
        }
        // ------------------------------------ 

        // --- RESET UI AND GAME STATE ---
        // Clear history and captured pieces from the last game.
        MoveHistory.Instance.ClearHistory();
        GameplayUI.Instance.ClearLastMoveDisplay();
        PieceCaptureManager.Instance.ClearCapturedLists();
        GameplayUI.Instance.ClearCapturedPieceUI();

        // Reset the board visuals and piece positions
        Chessboard.Instance.GenerateBoard();
        ChessPieceManager.Instance.SpawnAllPieces();

        // Reset the timers
        TurnManager.Instance.StartNewGame();

        // Tell the UIManager to switch to the gameplay view.
        UIManager.Instance.ShowGameplayPanel();

        ChangeState(GameState.Playing);
        Debug.Log($"New game started in {CurrentGameMode} mode. It's White's turn.");
        
        BoardPresenter.Instance.OrientBoardToPlayer(true);
        NotifyCurrentPlayer();
    }

    // This method is called after every turn switch
    public void NotifyCurrentPlayer()
    {
        if (CurrentState != GameState.Playing) return;

        Player currentPlayer = TurnManager.Instance.IsWhiteTurn ? whitePlayer : blackPlayer;
        currentPlayer.OnTurnStart();
    }

    public void EndGame(GameState finalState, bool winnerIsWhite)
    {
        if (CurrentState == GameState.Playing)
        {
            ChangeState(finalState);
            TurnManager.Instance.StopTimer();
            AudioManager.Instance.PlayGameEndSound(finalState);
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
        else if (MoveValidator.Instance.HasInsufficientMaterial())
        {
            EndGame(GameState.Draw, false);
        }
        else if (MoveValidator.Instance.IsInCheck(currentPlayerIsWhite))
        {
            AudioManager.Instance.PlayCheckSound();
        }
    }

    public void InitiatePawnPromotion(ChessPiece pawn)
    {
        _pawnToPromote = pawn;
        ChangeState(GameState.Promotion);
        UIManager.Instance.ShowPromotionPanel(pawn.IsWhite);
    }

    public void FinalizePawnPromotion(PieceType newPieceType)
    {
        if (_pawnToPromote == null) return;

        ChessPieceManager.Instance.PromotePawn(_pawnToPromote, newPieceType);
        _pawnToPromote = null;

        ChangeState(GameState.Playing);
        UIManager.Instance.HidePromotionPanel();
        AudioManager.Instance.PlayPromotionSound();
        CheckForGameEnd();
    }

    private void ChangeState(GameState newState)
    {
        CurrentState = newState;
        OnGameStateChanged?.Invoke(newState);
    }

    // --- HELPER METHODS FOR PLAYER ACCESS ---
    public Player GetCurrentPlayer()
    {
        return TurnManager.Instance.IsWhiteTurn ? whitePlayer : blackPlayer;
    }

    public Player GetWhitePlayer()
    {
        return whitePlayer;
    }

    public Player GetBlackPlayer()
    {
        return blackPlayer;
    }
    // ----------------------------------------
}