using System.Collections.Generic;
using UnityEngine;

public class PawnPiece : ChessPiece
{
    public override List<Vector2Int> GetPossibleMoves(Chessboard board)
    {
        var moves = new List<Vector2Int>();
        
        int direction = IsWhite ? 1 : -1;
        
        // --- Single Step Forward ---
        Vector2Int forwardMove = new Vector2Int(_boardPosition.x, _boardPosition.y + direction);
        if (board.GetPieceAt(forwardMove) == null)
        {
            moves.Add(forwardMove);
            
            // --- Double Step Forward (only on first move) ---
            if (!_hasMoved)
            {
                Vector2Int doubleForwardMove = new Vector2Int(_boardPosition.x, _boardPosition.y + (2 * direction));
                if (board.GetPieceAt(doubleForwardMove) == null)
                {
                    moves.Add(doubleForwardMove);
                }
            }
        }
        
        // --- Diagonal Captures ---
        // Capture Right
        Vector2Int captureRight = new Vector2Int(_boardPosition.x + 1, _boardPosition.y + direction);
        ChessPiece pieceRight = board.GetPieceAt(captureRight);
        if (pieceRight != null && pieceRight.IsWhite != this.IsWhite)
        {
            moves.Add(captureRight);
        }

        // Capture Left
        Vector2Int captureLeft = new Vector2Int(_boardPosition.x - 1, _boardPosition.y + direction);
        ChessPiece pieceLeft = board.GetPieceAt(captureLeft);
        if (pieceLeft != null && pieceLeft.IsWhite != this.IsWhite)
        {
            moves.Add(captureLeft);
        }

        return moves;
    }
}