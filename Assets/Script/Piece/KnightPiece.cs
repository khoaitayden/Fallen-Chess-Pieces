using System.Collections.Generic;
using UnityEngine;

public class KnightPiece : ChessPiece, IClickable
{
    // This helper function replaces the need for IsValidSquare()
    private bool IsOnBoard(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < Constants.BOARD_SIZE && pos.y >= 0 && pos.y < Constants.BOARD_SIZE;
    }

    public override List<Vector2Int> GetAttackMoves(BoardState boardState)
    {
        return GetPossibleMoves(boardState);
    }

    public override List<Vector2Int> GetPossibleMoves(BoardState boardState)
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

            if (!IsOnBoard(nextPos))
            {
                continue;
            }

            // Get the piece data directly from the array.
            var pieceAtTarget = boardState.Pieces[nextPos.x, nextPos.y];

            if (pieceAtTarget == null || pieceAtTarget.Value.IsWhite != this.IsWhite)
            {
                moves.Add(nextPos);
            }
        }

        return moves;
    }
}