using System.Collections.Generic;
using UnityEngine;

public class RookPiece : ChessPiece, IClickable
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

        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(0, 1), new Vector2Int(0, -1),
            new Vector2Int(1, 0), new Vector2Int(-1, 0)
        };

        foreach (var direction in directions)
        {
            CheckDirection(moves, boardState, direction);
        }

        return moves;
    }

    private void CheckDirection(List<Vector2Int> moves, BoardState boardState, Vector2Int direction)
    {
        Vector2Int nextPos = _boardPosition + direction;

        while (IsOnBoard(nextPos))
        {
            // Get the piece data directly from the array.
            var pieceAtTarget = boardState.Pieces[nextPos.x, nextPos.y];

            if (pieceAtTarget == null)
            {
                moves.Add(nextPos);
                nextPos += direction;
            }
            else if (pieceAtTarget.Value.IsWhite != this.IsWhite)
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