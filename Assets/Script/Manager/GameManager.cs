using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameMode CurrentGameMode { get; private set; }
    public GameState CurrentState { get; private set; }

    private Player whitePlayer;
    private Player blackPlayer;

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
    public void StartNewGame(GameMode mode, AIDifficulty difficulty = AIDifficulty.Easy)
    {
        CurrentGameMode = mode;

        switch (CurrentGameMode)
        {
            case GameMode.Local:
                whitePlayer = new HumanPlayer(true);
                blackPlayer = new HumanPlayer(false);
                break;

            case GameMode.AI:
                IAIStrategy strategy;
                switch (difficulty)
                {
                    case AIDifficulty.Normal:
                        strategy = new NormalAIStrategy();
                        break;
                    case AIDifficulty.Hard:
                        strategy = new HardAIStrategy();
                        break;
                    case AIDifficulty.Easy:
                    default:
                        strategy = new EasyAIStrategy();
                        break;
                }
                whitePlayer = new HumanPlayer(true);
                blackPlayer = new AIPlayer(false, strategy);
                break;

            case GameMode.Online:
                whitePlayer = new HumanPlayer(true);
                blackPlayer = new HumanPlayer(false);
                break;
        }
        MoveHistory.Instance.ClearHistory();
        if (GameplayUI.Instance != null)
        {
            GameplayUI.Instance.ClearLastMoveDisplay();
            GameplayUI.Instance.ClearCapturedPieceUI();
        }
        PieceCaptureManager.Instance.ClearCapturedLists();
        
        Chessboard.Instance.GenerateBoard();
        ChessPieceManager.Instance.SpawnAllPieces();
        TurnManager.Instance.StartNewGame();

        UIManager.Instance.ShowGameplayPanel();
        ChangeState(GameState.Playing);
        
        Debug.Log($"New game started in {CurrentGameMode} mode (Difficulty: {difficulty}). It's White's turn.");
        
        BoardPresenter.Instance.OrientBoardToPlayer(true);
        KingPowerManager.Instance.ResetState();
        NotifyCurrentPlayer();
    }

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

        Player currentPlayer = GetCurrentPlayer();

        if (currentPlayer.Type == PlayerType.Human)
        {
            UIManager.Instance.ShowPromotionPanel(pawn.IsWhite);
        }
        else if (currentPlayer.Type == PlayerType.AI)
        {
            Debug.Log("AI is promoting a pawn to a Queen.");
            FinalizePawnPromotion(PieceType.Queen);
        }
    }

    public void FinalizePawnPromotion(PieceType newPieceType)
    {
        if (_pawnToPromote == null) return;

        ChessPieceManager.Instance.PromotePawn(_pawnToPromote, newPieceType);
        AudioManager.Instance.PlayPromotionSound();
        _pawnToPromote = null;

        ChangeState(GameState.Playing);
        if (UIManager.Instance != null) UIManager.Instance.HidePromotionPanel();

        CheckForGameEnd();

        if (CurrentState == GameState.Playing)
        {
            NotifyCurrentPlayer();
        }
    }

    private void ChangeState(GameState newState)
    {
        CurrentState = newState;
        OnGameStateChanged?.Invoke(newState);
    }

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
}