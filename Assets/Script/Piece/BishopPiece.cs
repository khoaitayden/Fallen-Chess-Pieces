using System.Collections.Generic;
using UnityEngine;

public class BishopPiece : ChessPiece
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