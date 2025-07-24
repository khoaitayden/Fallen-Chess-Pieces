// In GameManager.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public GameSettings CurrentSettings { get; private set; }
    public GameMode CurrentGameMode { get; private set; }
    public GameState CurrentState { get; private set; }
    
    private Player whitePlayer;
    private Player blackPlayer;
    public event System.Action<GameState> OnGameStateChanged;
    
    private ChessPiece _pawnToPromote;
    private PieceType _pendingPowerType;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); }
        else
        {
            Instance = this;
            CurrentSettings = new GameSettings();
        }
    }

    private void Start()
    {
        if (PowerManager.Instance != null)
        {
            PowerManager.Instance.OnPowerTransferRequired += HandlePowerTransferRequired;
        }
    }

    private void OnDestroy()
    {
        if (PowerManager.Instance != null)
        {
            PowerManager.Instance.OnPowerTransferRequired -= HandlePowerTransferRequired;
        }
    }

    // --- THIS IS THE NEW, CENTRAL GAME LOOP ---

    public void ProcessMove(ChessPiece piece, Vector2Int from, Vector2Int to)
    {
        // Tell the previous player to stop listening for input.
        GetCurrentPlayer()?.OnTurnEnd();

        // 1. Execute the move on the board.
        ChessPiece capturedPiece = Chessboard.Instance.MovePiece(piece, to);

        // 2. Log the move for history.
        LogMove(piece, from, to);

        // 3. Check for special states that PAUSE the game.
        if (piece.Type == PieceType.Pawn && (to.y == 0 || to.y == 7))
        {
            InitiatePawnPromotion(piece);
            return;
        }
        
        if (capturedPiece != null && PowerManager.Instance.CheckForPowerTransfer(capturedPiece))
        {
            return;
        }

        // 4. If it was a normal move, end the turn immediately.
        EndTurn();
    }

    public void EndTurn()
    {
        TurnManager.Instance.SwitchTurn();
        CheckForGameEnd();
        if (CurrentState == GameState.Playing)
        {
            NotifyCurrentPlayer();
        }
    }

    private void LogMove(ChessPiece piece, Vector2Int from, Vector2Int to)
    {
        string playerColorName = piece.IsWhite ? "White" : "Black";
        string logColor = piece.IsWhite ? "white" : "#808080";
        Debug.Log($"<color={logColor}><b>{playerColorName}</b> moved <b>{piece.Type}</b> from <b>{from}</b> to <b>{to}</b>.</color>");

        TurnManager.Instance.SetEnPassantTarget(piece, from, to);
        string notation = MoveConverter.ToDescriptiveNotation(piece, to);
        MoveData move = new MoveData(piece.Type, from, to, notation);
        MoveHistory.Instance.AddMove(move);
    }
    
    // --- SPECIAL STATE HANDLING ---

    private void HandlePowerTransferRequired(bool isWhite, PieceType powerType)
    {
        _pendingPowerType = powerType;
        ChangeState(GameState.PowerTransfer);
        UIManager.Instance.ShowPowerTransferPanel(isWhite, powerType);
        
        Player playerToChoose = isWhite ? GetWhitePlayer() : GetBlackPlayer();
        BoardPresenter.Instance.OrientBoardToPlayer(isWhite);

        if (playerToChoose is HumanPlayer humanPlayer)
        {
            humanPlayer.EnablePowerTransferInput();
            humanPlayer.HighlightPowerTargets();
        }
        else if (playerToChoose is AIPlayer)
        {
            ChessPiece targetPiece = FindBestAIPowerTarget(isWhite);
            if (targetPiece != null)
            {
                CompletePowerTransfer(targetPiece);
            }
            else
            {
                ResumeTurnAfterChoice(null);
            }
        }
    }

    public void CompletePowerTransfer(ChessPiece targetPiece)
    {
        PowerManager.Instance.GrantPower(targetPiece, _pendingPowerType);
        ResumeTurnAfterChoice(targetPiece);
    }

    public void FinalizePawnPromotion(PieceType newPieceType)
    {
        if (_pawnToPromote == null) return;
        ChessPieceManager.Instance.PromotePawn(_pawnToPromote, newPieceType);
        _pawnToPromote = null;
        ResumeTurnAfterChoice(null);
    }
    
    private void ResumeTurnAfterChoice(ChessPiece pieceInvolved)
    {
        if (pieceInvolved != null)
        {
            Player playerWhoChose = pieceInvolved.IsWhite ? GetWhitePlayer() : GetBlackPlayer();
            if (playerWhoChose is HumanPlayer humanPlayer)
            {
                humanPlayer.DisablePowerTransferInput();
            }
        }
        
        ChangeState(GameState.Playing);
        UIManager.Instance.HidePowerTransferPanel();
        UIManager.Instance.HidePromotionPanel();
        
        // The choice was part of the turn. NOW the turn can end.
        EndTurn();
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
                    case AIDifficulty.Normal: strategy = new NormalAIStrategy(); break;
                    case AIDifficulty.Hard:   strategy = new HardAIStrategy();   break;
                    default:                  strategy = new EasyAIStrategy();   break;
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
        PowerManager.Instance.ResetState();
        Chessboard.Instance.GenerateBoard();
        ChessPieceManager.Instance.SpawnAllPieces();
        TurnManager.Instance.StartNewGame();
        
        UIManager.Instance.ShowGameplayPanel();
        ChangeState(GameState.Playing);
        Debug.Log($"New game started in {CurrentGameMode} mode (Difficulty: {difficulty}). It's White's turn.");
        BoardPresenter.Instance.OrientBoardToPlayer(true);
        NotifyCurrentPlayer();
    }

    public void NotifyCurrentPlayer()
    {
        if (CurrentState != GameState.Playing) return;
        Player currentPlayer = TurnManager.Instance.IsWhiteTurn ? whitePlayer : blackPlayer;
        currentPlayer?.OnTurnStart();
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

    public void EndGame(GameState finalState, bool winnerIsWhite)
    {
        if (CurrentState == GameState.Playing)
        {
            ChangeState(finalState);
            TurnManager.Instance.StopTimer();
            AudioManager.Instance.PlayGameEndSound(finalState);
        }
    }

    private void ChangeState(GameState newState)
    {
        CurrentState = newState;
        OnGameStateChanged?.Invoke(newState);
    }

    private ChessPiece FindBestAIPowerTarget(bool isWhite)
    {
        List<ChessPiece> validTargets = new List<ChessPiece>();
        for (int x = 0; x < Constants.BOARD_SIZE; x++)
        {
            for (int y = 0; y < Constants.BOARD_SIZE; y++)
            {
                ChessPiece piece = Chessboard.Instance.GetPieceAt(new Vector2Int(x, y));
                if (piece != null && piece.IsWhite == isWhite &&
                    piece.Type != PieceType.King && piece.Type != PieceType.Queen && piece.Type != PieceType.Pawn)
                {
                    validTargets.Add(piece);
                }
            }
        }
        if (validTargets.Count == 0) return null;
        return validTargets[Random.Range(0, validTargets.Count)];
    }

    public Player GetCurrentPlayer() => TurnManager.Instance.IsWhiteTurn ? whitePlayer : blackPlayer;
    public Player GetWhitePlayer() => whitePlayer;
    public Player GetBlackPlayer() => blackPlayer;
}