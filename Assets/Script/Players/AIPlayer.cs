using UnityEngine;
using System.Threading.Tasks;

public class AIPlayer : Player
{
    private Chessboard _chessboard;
    private IAIStrategy _strategy;
    private bool _isThinking = false; 

    public AIPlayer(bool isWhite, IAIStrategy strategy) : base(isWhite, PlayerType.AI) 
    {
        _chessboard = Object.FindObjectOfType<Chessboard>();
        _strategy = strategy;
    }

    public override void OnTurnStart()
    {
        if (GameManager.Instance.CurrentState == GameState.Playing && !_isThinking)
        {
            ThinkAndMakeMove();
        }
    }

    public override void OnTurnEnd()
    {
        _isThinking = false;
    }

    private async void ThinkAndMakeMove()
    {
        if (_isThinking) return;
        _isThinking = true;

        MoveData bestMove = await Task.Run(() => _strategy.GetBestMove(this.IsWhite, _chessboard));

        if (!_isThinking || GameManager.Instance.CurrentState != GameState.Playing)
        {
            _isThinking = false;
            return;
        }

        if (bestMove.Equals(default(MoveData)))
        {
            _isThinking = false;
            GameManager.Instance.EndTurn();
            return;
        }

        ChessPiece pieceToMove = _chessboard.GetPieceAt(bestMove.From);
        if (pieceToMove == null)
        {
            _isThinking = false;
            GameManager.Instance.EndTurn();
            return;
        }
        
        GameManager.Instance.ProcessMove(pieceToMove, bestMove.From, bestMove.To);
        _isThinking = false;
    }
}