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


    public void ProcessMove(ChessPiece piece, Vector2Int from, Vector2Int to)
    {
        bool isWhiteMove = piece.IsWhite;
        if (TurnManager.Instance.IsWhiteTurn != isWhiteMove)
        {
            Debug.LogWarning($"Ignoring move: it's not {(isWhiteMove ? "White" : "Black")}'s turn!");
            return;
        }
        GetCurrentPlayer()?.OnTurnEnd();

        ChessPiece capturedPiece = Chessboard.Instance.MovePiece(piece, to);

        LogMove(piece, from, to);

        if (piece.Type == PieceType.Pawn && (to.y == 0 || to.y == 7))
        {
            InitiatePawnPromotion(piece);
            return;
        }

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
    
    private void HandlePowerTransferRequired(bool isWhite, PieceType powerType)
    {
        if (GetCurrentPlayer() is HumanPlayer h) {
            h.DisablePowerTransferInput();
            h.ClearAllHighlights();
        }
        UIManager.Instance.HidePowerTransferPanel();

        List<ChessPiece> validTargets = FindValidPowerTargets(isWhite);
        if (validTargets.Count == 0)
        {
            Debug.Log($"No valid power targets.  Skipping transfer for {(isWhite?"White":"Black")}.");
            ChangeState(GameState.Playing);
            EndTurn();
            return;
        }

        _pendingPowerType = powerType;
        ChangeState(GameState.PowerTransfer);
        UIManager.Instance.ShowPowerTransferPanel(isWhite, powerType);
        
        Player playerToChoose = isWhite ? GetWhitePlayer() : GetBlackPlayer();
        if (playerToChoose is HumanPlayer humanPlayer)
        {
            humanPlayer.ClearAllHighlights();
            humanPlayer.EnablePowerTransferInput();
            humanPlayer.HighlightPowerTargets();
        }
        // --- THIS IS THE FIX ---
        else if (playerToChoose is AIPlayer)
        {
            // The AI chooses instantly. We grant the power and let the
            // original ProcessMove method continue to the EndTurn() call.
            // We DO NOT call CompletePowerTransfer or ResumeTurnAfterChoice here.
            ChessPiece targetPiece = validTargets[Random.Range(0, validTargets.Count)];
            PowerManager.Instance.GrantPower(targetPiece, _pendingPowerType);
            
            // The choice has been made, so the state can return to normal.
            ChangeState(GameState.Playing); 
            UIManager.Instance.HidePowerTransferPanel();
        }
    }

    public List<ChessPiece> FindValidPowerTargets(bool isWhite)
    {
        List<ChessPiece> validTargets = new List<ChessPiece>();
        
        for (int x = 0; x < Constants.BOARD_SIZE; x++)
        {
            for (int y = 0; y < Constants.BOARD_SIZE; y++)
            {
                ChessPiece piece = Chessboard.Instance.GetPieceAt(new Vector2Int(x, y));
                if (piece != null && 
                    piece.IsWhite == isWhite &&
                    piece.Type != PieceType.King && 
                    piece.Type != PieceType.Queen && 
                    piece.Type != PieceType.Pawn)
                {
                    validTargets.Add(piece);
                }
            }
        }
        
        return validTargets;
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

        ChangeState(GameState.Playing);
        UIManager.Instance.HidePromotionPanel();
        if (GetCurrentPlayer() is HumanPlayer hp) hp.ClearAllHighlights();

        EndTurn();
    }
    
    private void ResumeTurnAfterChoice(ChessPiece pieceInvolved)
    {
        if (pieceInvolved != null && GetCurrentPlayer() is HumanPlayer human)
        {
            human.DisablePowerTransferInput();
            human.ClearAllHighlights();
        }

        ChangeState(GameState.Playing);
        UIManager.Instance.HidePowerTransferPanel();
        UIManager.Instance.HidePromotionPanel();
        
        NotifyCurrentPlayer();
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