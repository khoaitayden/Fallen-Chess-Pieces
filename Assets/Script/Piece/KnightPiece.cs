using System.Collections.Generic;
using UnityEngine;

public class KnightPiece : ChessPiece, IClickable
{
    public override List<Vector2Int> GetAttackMoves(Chessboard board)
    {
        return GetPossibleMoves(board);
    }

    public override List<Vector2Int> GetPossibleMoves(Chessboard board)
    {
        var moves = new List<Vector2Int>();

        Vector2Int[] offsets = new Vector2Int[]
        {
            new Vector2Int(1, 2), new Vector2Int(1, -2),
            new Vector2Int(-1, 2), new Vector2Int(-1, -2),
            new Vector2Int(2, 1), new Vector2Int(2, -1),
            new Vector2Int(-2, 1), new Vector2Int(-2, -1)
        };

        foreach (var offset in offsets)
        {
            Vector2Int nextPos = _boardPosition + offset;

            if (board.GetSquareAt(nextPos) == null)
            {
                continue;
            }

            ChessPiece pieceAtNextPos = board.GetPieceAt(nextPos);

            if (pieceAtNextPos == null || pieceAtNextPos.IsWhite != this.IsWhite)
            {
                moves.Add(nextPos);
            }
        }

        return moves;
    }
}