using System.Collections.Generic;
using UnityEngine;

public class KingPiece : ChessPiece
{
    public override List<Vector2Int> GetAttackMoves(Chessboard board)
    {
        var moves = new List<Vector2Int>();
        Vector2Int currentPos = _boardPosition;

        // Standard 8-direction King moves
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                Vector2Int nextPos = new Vector2Int(currentPos.x + x, currentPos.y + y);
                if (board.GetSquareAt(nextPos) != null)
                {
                    // For attack moves, we don't care if the piece is friendly or not
                    // We just care that the king can "see" that square
                    moves.Add(nextPos);
                }
            }
        }
        return moves;
    }
    public override List<Vector2Int> GetPossibleMoves(Chessboard board)
    {
        var moves = new List<Vector2Int>();
        Vector2Int currentPos = _boardPosition;

        // Standard 8-direction King moves
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                Vector2Int nextPos = new Vector2Int(currentPos.x + x, currentPos.y + y);
                if (board.GetSquareAt(nextPos) != null)
                {
                    ChessPiece piece = board.GetPieceAt(nextPos);
                    if (piece == null || piece.IsWhite != this.IsWhite)
                    {
                        moves.Add(nextPos);
                    }
                }
            }
        }

        if (!_hasMoved)
        {
            // Check Kingside
            if (MoveValidator.Instance.CanCastleKingside(this))
            {
                moves.Add(new Vector2Int(_boardPosition.x + 2, _boardPosition.y));
            }
            // Check Queenside
            if (MoveValidator.Instance.CanCastleQueenside(this))
            {
                moves.Add(new Vector2Int(_boardPosition.x - 2, _boardPosition.y));
            }
        }
        return moves;
    }
}