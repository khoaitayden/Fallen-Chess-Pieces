using System.Collections.Generic;
using UnityEngine;

public class QueenPiece : ChessPiece
{
    public override List<Vector2Int> GetPossibleMoves(Chessboard board)
    {
        var moves = new List<Vector2Int>();

        // --- Straight directions---
        Vector2Int[] straightDirections = new Vector2Int[]
        {
            new Vector2Int(0, 1),   
            new Vector2Int(0, -1),  
            new Vector2Int(-1, 0),  
            new Vector2Int(1, 0)    
        };

        // --- Diagonal directions---
        Vector2Int[] diagonalDirections = new Vector2Int[]
        {
            new Vector2Int(1, 1),  
            new Vector2Int(1, -1),  
            new Vector2Int(-1, -1), 
            new Vector2Int(-1, 1)  
        };

        // Combine both sets of directions
        foreach (var direction in straightDirections)
        {
            CheckDirection(moves, board, direction);
        }

        foreach (var direction in diagonalDirections)
        {
            CheckDirection(moves, board, direction);
        }

        return moves;
    }

    private void CheckDirection(List<Vector2Int> moves, Chessboard board, Vector2Int direction)
    {
        Vector2Int nextPos = _boardPosition + direction;

        while (true)
        {
            if (board.GetSquareAt(nextPos) == null)
                break;

            ChessPiece pieceAtNextPos = board.GetPieceAt(nextPos);

            if (pieceAtNextPos == null)
            {
                moves.Add(nextPos);
                nextPos += direction;
            }
            else if (pieceAtNextPos.IsWhite != this.IsWhite)
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