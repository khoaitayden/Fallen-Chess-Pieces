// In AIPlayer.cs
using System.Collections;
using UnityEngine;
using System.Threading.Tasks; // Required for running on a separate thread

public class AIPlayer : Player
{
    private Chessboard _chessboard;
    private IAIStrategy _strategy;

    public AIPlayer(bool isWhite, IAIStrategy strategy) : base(isWhite, PlayerType.AI) 
    {
        _chessboard = Object.FindObjectOfType<Chessboard>();
        _strategy = strategy;
    }

    public override void OnTurnStart()
    {
        // We no longer use a coroutine. We start the threaded task directly.
        ThinkAndMakeMove();
    }

    private async void ThinkAndMakeMove()
    {
        Debug.Log("AI Player's turn. Starting background task to think...");

        // --- RUN ON A BACKGROUND THREAD ---
        // Task.Run() executes the provided code on a thread pool thread.
        // The 'await' keyword pauses this method without freezing the game,
        // waiting for the background task to complete.
        MoveData bestMove = await Task.Run(() => _strategy.GetBestMove(this.IsWhite, _chessboard));
        // ---------------------------------

        Debug.Log("AI has finished thinking. Executing move on main thread.");

        // --- EXECUTE ON THE MAIN THREAD ---
        // All Unity API calls (like moving GameObjects) MUST happen on the main thread.
        // By this point, the 'await' is finished, and we are back on the main thread.

        if (bestMove.Equals(default(MoveData)))
        {
            Debug.LogWarning("AI strategy could not find a valid move.");
            // Potentially force a game end here if this happens (e.g., stalemate)
            return;
        }

        ChessPiece pieceToMove = _chessboard.GetPieceAt(bestMove.From);
        _chessboard.MovePiece(pieceToMove, bestMove.To);
        TurnManager.Instance.SetEnPassantTarget(pieceToMove, bestMove.From, bestMove.To);
        TurnManager.Instance.SwitchTurn();

        // We need to re-generate the notation here as it was empty in the AI's MoveData
        bool isCheck = MoveValidator.Instance.IsInCheck(TurnManager.Instance.IsWhiteTurn);
        bool isCheckmate = MoveValidator.Instance.IsCheckmate(TurnManager.Instance.IsWhiteTurn);
        string notation = MoveConverter.ToDescriptiveNotation(pieceToMove, bestMove.To);
        MoveData finalMove = new MoveData(bestMove.Piece, bestMove.From, bestMove.To, notation);
        
        MoveHistory.Instance.AddMove(finalMove);
        GameManager.Instance.CheckForGameEnd();

        if (GameManager.Instance.CurrentState == GameState.Playing)
        {
            GameManager.Instance.NotifyCurrentPlayer();
        }
    }
}