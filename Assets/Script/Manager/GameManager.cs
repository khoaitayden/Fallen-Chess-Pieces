using UnityEngine;
using System.Collections;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameMode CurrentGameMode { get; private set; }
    public GameState CurrentState { get; private set; }

    private Player whitePlayer;
    private Player blackPlayer;
    private LocalPlayerController _localController; // NEW: For local mode

    public event System.Action<GameState> OnGameStateChanged;
    private ChessPiece _pawnToPromote;

    private void Awake()
    {
        Instance = this;
        _localController = GetComponentInChildren<LocalPlayerController>(true);
    }

    private void Start() { } // Waits for UI command

    public void StartLocalGame()
    {
        Debug.Log("Starting new Local (Player vs Player) game.");
        CurrentGameMode = GameMode.Local;
        whitePlayer = null; // Not used in local mode
        blackPlayer = null;
        
        SetupNewGameBoard();
        
        _localController.gameObject.SetActive(true); // Activate the local controller
        BoardPresenter.Instance.OrientBoardToPlayer(true);
    }

    public void StartAIGame(AIDifficulty difficulty)
    {
        Debug.Log($"Starting new AI game with difficulty: {difficulty}");
        CurrentGameMode = GameMode.AI;
        _localController.gameObject.SetActive(false); // Deactivate local controller

        IAIStrategy strategy;
        switch (difficulty)
        {
            case AIDifficulty.Normal: strategy = new NormalAIStrategy(); break;
            case AIDifficulty.Hard:   strategy = new HardAIStrategy();   break;
            default:                  strategy = new EasyAIStrategy();   break;
        }
        whitePlayer = new HumanPlayer(true);
        blackPlayer = new AIPlayer(false, strategy);

        SetupNewGameBoard();
        BoardPresenter.Instance.OrientBoardToPlayer(true);
        NotifyCurrentPlayer();
    }

    public void StartOnlineGame(bool iAmWhite, NetworkMoveRelay localPlayerRelay)
    {
        // Use a coroutine to wait until all singletons are initialized.
        StartCoroutine(StartOnlineGameRoutine(iAmWhite, localPlayerRelay));
    }

    private IEnumerator StartOnlineGameRoutine(bool iAmWhite, NetworkMoveRelay localPlayerRelay)
    {
        // Wait until all necessary singletons are no longer null.
        yield return new WaitUntil(() => MoveHistory.Instance != null && GameplayUI.Instance != null && PieceCaptureManager.Instance != null);

        Debug.Log($"Starting new Online game. I am {(iAmWhite ? "White" : "Black")}.");
        CurrentGameMode = GameMode.Online;

        if (iAmWhite)
        {
            whitePlayer = new HumanPlayer(true, localPlayerRelay);
            blackPlayer = new OnlinePlayer(false);
        }
        else
        {
            whitePlayer = new OnlinePlayer(true);
            blackPlayer = new HumanPlayer(false, localPlayerRelay);
        }

        SetupNewGameBoard();
        BoardPresenter.Instance.OrientBoardToPlayer(iAmWhite);
        
        if (whitePlayer.Type == PlayerType.Human) whitePlayer.OnTurnStart();
        else whitePlayer.OnTurnStart();
    }

    private void SetupNewGameBoard()
    {
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
        Debug.Log($"Game setup complete. Mode: {CurrentGameMode}. It's White's turn.");
    }

    public void NotifyCurrentPlayer()
    {
        if (CurrentState != GameState.Playing) return;
        if (CurrentGameMode == GameMode.Local) return; // Local mode is handled by its own controller

        Player currentPlayer = TurnManager.Instance.IsWhiteTurn ? whitePlayer : blackPlayer;
        currentPlayer.OnTurnStart();
    }

    public void EndGame(GameState finalState, bool winnerIsWhite)
    {
        if (CurrentState == GameState.Playing || CurrentState == GameState.Promotion)
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
            EndGame(GameState.Checkmate, !currentPlayerIsWhite);
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

    public Player GetCurrentPlayer() => TurnManager.Instance.IsWhiteTurn ? whitePlayer : blackPlayer;
    public Player GetWhitePlayer() => whitePlayer;
    public Player GetBlackPlayer() => blackPlayer;
}