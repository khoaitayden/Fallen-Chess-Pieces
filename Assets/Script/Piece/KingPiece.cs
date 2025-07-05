using System.Collections.Generic;
using UnityEngine;

public class KingPiece : ChessPiece, IClickable
{
    // This helper function replaces the need for IsValidSquare()
    private bool IsOnBoard(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < Constants.BOARD_SIZE && pos.y >= 0 && pos.y < Constants.BOARD_SIZE;
    }

    public override List<Vector2Int> GetAttackMoves(BoardState boardState)
    {
        var moves = new List<Vector2Int>();
        Vector2Int currentPos = _boardPosition;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                Vector2Int nextPos = new Vector2Int(currentPos.x + x, currentPos.y + y);

                if (!IsOnBoard(nextPos))
                    continue;

                moves.Add(nextPos);
            }
        }

        return moves;
    }

    public override List<Vector2Int> GetPossibleMoves(BoardState boardState)
    {
        var moves = new List<Vector2Int>();
        Vector2Int currentPos = _boardPosition;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                Vector2Int nextPos = new Vector2Int(currentPos.x + x, currentPos.y + y);

                if (!IsOnBoard(nextPos))
                    continue;

                // Get the piece data directly from the array.
                var pieceAtTarget = boardState.Pieces[nextPos.x, nextPos.y];

                if (pieceAtTarget == null || pieceAtTarget.Value.IsWhite != this.IsWhite)
                {
                    moves.Add(nextPos);
                }
            }
        }

        // Note: Castling logic might need to be adjusted for BoardState
        // You may need to pass additional information to BoardState or handle this differently
        if (!_hasMoved)
        {
            if (MoveValidator.Instance.CanCastleKingside(this))
            {
                moves.Add(new Vector2Int(currentPos.x + 2, currentPos.y));
            }
            if (MoveValidator.Instance.CanCastleQueenside(this))
            {
                moves.Add(new Vector2Int(currentPos.x - 2, currentPos.y));
            }
        }

        return moves;
    }
}