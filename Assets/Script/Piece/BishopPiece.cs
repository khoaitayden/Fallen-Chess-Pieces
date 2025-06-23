using System.Collections.Generic;
using UnityEngine;

public class BishopPiece : ChessPiece
{
    public override List<Vector2Int> GetPossibleMoves(Chessboard board)
    {
        var moves = new List<Vector2Int>();

        // The four diagonal directions: NE, SE, SW, NW
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(1, 1),   // Northeast
            new Vector2Int(1, -1),  // Southeast
            new Vector2Int(-1, -1), // Southwest
            new Vector2Int(-1, 1)   // Northwest
        };

        foreach (var direction in directions)
        {
            CheckDirection(moves, board, direction);
        }

        return moves;
    }

    // Helper method to check all squares in a diagonal direction
    private void CheckDirection(List<Vector2Int> moves, Chessboard board, Vector2Int direction)
    {
        Vector2Int nextPos = _boardPosition + direction;

        while (true)
        {
            // Stop if we're out of bounds
            if (board.GetSquareAt(nextPos) == null)
                break;

            ChessPiece pieceAtNextPos = board.GetPieceAt(nextPos);

            // Empty square is a valid move
            if (pieceAtNextPos == null)
            {
                moves.Add(nextPos);
                nextPos += direction;
            }
            // Enemy piece — valid capture, but stop here
            else if (pieceAtNextPos.IsWhite != this.IsWhite)
            {
                moves.Add(nextPos);
                break;
            }
            // Friendly piece — blocked, stop here
            else
            {
                break;
            }
        }
    }
}