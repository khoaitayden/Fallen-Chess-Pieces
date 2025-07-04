using System.Collections.Generic;
using UnityEngine;

public class BishopPiece : ChessPiece, IClickable
{
    public override List<Vector2Int> GetAttackMoves(Chessboard board)
    {
        return GetPossibleMoves(board);
    }

    public override List<Vector2Int> GetPossibleMoves(Chessboard board)
    {
        var moves = new List<Vector2Int>();

        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(1, 1),
            new Vector2Int(1, -1),
            new Vector2Int(-1, -1),
            new Vector2Int(-1, 1)
        };

        foreach (var direction in directions)
        {
            CheckDirection(moves, board, direction);
        }

        return moves;
    }

    private void CheckDirection(List<Vector2Int> moves, Chessboard board, Vector2Int direction)
    {
        Vector2Int nextPos = _boardPosition + direction;

        while (board.GetSquareAt(nextPos) != null)
        {
            ChessPiece piece = board.GetPieceAt(nextPos);

            if (piece == null)
            {
                moves.Add(nextPos);
                nextPos += direction;
            }
            else if (piece.IsWhite != this.IsWhite)
            {
                moves.Add(nextPos);
                break;
            }
            else
            {
                break;
            }
        }
    }
}