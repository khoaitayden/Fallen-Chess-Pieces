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
        if (GameManager.Instance.CurrentState == GameState.Playing)
        {
            ThinkAndMakeMove();
        }
    }

    private async void ThinkAndMakeMove()
    {
        MoveData bestMove = await Task.Run(() => _strategy.GetBestMove(this.IsWhite, _chessboard));

        if (bestMove.Equals(default(MoveData)))
        {
            GameManager.Instance.EndTurn();
            return;
        }

        ChessPiece pieceToMove = _chessboard.GetPieceAt(bestMove.From);
        if (pieceToMove == null)
        {
            GameManager.Instance.EndTurn();
            return;
        }
        
        GameManager.Instance.ProcessMove(pieceToMove, bestMove.From, bestMove.To);
    }
}