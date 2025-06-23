using System.Collections.Generic;
using UnityEngine;

public class KingPiece : ChessPiece
{
    public override List<Vector2Int> GetPossibleMoves(Chessboard board)
    {
        var moves = new List<Vector2Int>();

        // All 8 possible directions the king can move
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(0, 1),   // Up
            new Vector2Int(1, 1),   // Up-Right
            new Vector2Int(1, 0),   // Right
            new Vector2Int(1, -1),  // Down-Right
            new Vector2Int(0, -1),  // Down
            new Vector2Int(-1, -1), // Down-Left
            new Vector2Int(-1, 0),  // Left
            new Vector2Int(-1, 1)   // Up-Left
        };

        foreach (var direction in directions)
        {
            Vector2Int nextPos = _boardPosition + direction;

            // Check if the position is on the board
            if (board.GetSquareAt(nextPos) == null)
                continue;

            ChessPiece pieceAtNextPos = board.GetPieceAt(nextPos);

            // Allow move if square is empty or has an enemy piece
            if (pieceAtNextPos == null || pieceAtNextPos.IsWhite != IsWhite)
            {
                moves.Add(nextPos);
            }
        }

        return moves;
    }
}