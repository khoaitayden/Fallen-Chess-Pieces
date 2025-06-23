using System.Collections.Generic;
using UnityEngine;

public class KnightPiece : ChessPiece
{
    public override List<Vector2Int> GetPossibleMoves(Chessboard board)
    {
        var moves = new List<Vector2Int>();

        // All 8 possible knight moves (L-shaped)
        Vector2Int[] moveDirections = new Vector2Int[]
        {
            new Vector2Int(1, 2),
            new Vector2Int(2, 1),
            new Vector2Int(2, -1),
            new Vector2Int(1, -2),
            new Vector2Int(-1, -2),
            new Vector2Int(-2, -1),
            new Vector2Int(-2, 1),
            new Vector2Int(-1, 2)
        };

        foreach (var direction in moveDirections)
        {
            Vector2Int nextPos = _boardPosition + direction;

            // Check if the position is within bounds
            if (board.GetSquareAt(nextPos) == null)
                continue;

            ChessPiece pieceAtNextPos = board.GetPieceAt(nextPos);

            // If square is empty or has an enemy piece
            if (pieceAtNextPos == null || pieceAtNextPos.IsWhite != IsWhite)
            {
                moves.Add(nextPos);
            }
        }

        return moves;
    }
}