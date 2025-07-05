using System.Collections;
using UnityEngine;
using System.Threading.Tasks;

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
        ThinkAndMakeMove();
    }

    private async void ThinkAndMakeMove()
    {
        Debug.Log("AI Player's turn. Starting background task to think...");
        MoveData bestMove = await Task.Run(() => _strategy.GetBestMove(this.IsWhite, _chessboard));
        Debug.Log("AI has finished thinking. Executing move on main thread.");

        if (bestMove.Equals(default(MoveData)))
        {
            Debug.LogWarning("AI strategy could not find a valid move.");
            return;
        }

        ChessPiece pieceToMove = _chessboard.GetPieceAt(bestMove.From);

        bool isPromotion = (pieceToMove.Type == PieceType.Pawn &&
                        (pieceToMove.IsWhite && bestMove.To.y == 7 ||
                        !pieceToMove.IsWhite && bestMove.To.y == 0));

        _chessboard.MovePiece(pieceToMove, bestMove.To);

        if (isPromotion)
        {
            return;
        }

        TurnManager.Instance.SetEnPassantTarget(pieceToMove, bestMove.From, bestMove.To);
        TurnManager.Instance.SwitchTurn();

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