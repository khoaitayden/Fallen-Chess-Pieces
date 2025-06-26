using System.Collections.Generic;
using UnityEngine;

public class PawnPiece : ChessPiece
{
    public override List<Vector2Int> GetAttackMoves(Chessboard board)
    {
        var moves = new List<Vector2Int>();
        int direction = IsWhite ? 1 : -1;

        // A pawn only attacks the two diagonal squares in front of it.
        Vector2Int captureRight = new Vector2Int(_boardPosition.x + 1, _boardPosition.y + direction);
        if (board.GetSquareAt(captureRight) != null)
        {
            moves.Add(captureRight);
        }

        Vector2Int captureLeft = new Vector2Int(_boardPosition.x - 1, _boardPosition.y + direction);
        if (board.GetSquareAt(captureLeft) != null)
        {
            moves.Add(captureLeft);
        }

        return moves;
    }
    public override List<Vector2Int> GetPossibleMoves(Chessboard board)
    {
        var moves = new List<Vector2Int>();
        int direction = IsWhite ? 1 : -1;

        // --- Single Step Forward ---
        Vector2Int forwardMove = new Vector2Int(_boardPosition.x, _boardPosition.y + direction);
        // --- Double Step Forward ---
        Vector2Int doubleForwardMove = new Vector2Int(_boardPosition.x, _boardPosition.y + (2 * direction));
        if (board.GetPieceAt(forwardMove) == null)
        {
            moves.Add(forwardMove);
            //Check if can move forward two squares
            if (!_hasMoved)
            {
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

        //EN PASSANT
        Vector2Int enPassantTarget = TurnManager.Instance.EnPassantTargetSquare;
        if (enPassantTarget != new Vector2Int(-1, -1))
        {
            // Check if pawn is on the correct rank for en passant
            if ((_boardPosition.y == 4 && IsWhite) || (_boardPosition.y == 3 && !IsWhite))
            {
                // Check if the en passant target is directly diagonally adjacent
                if ((enPassantTarget.x == _boardPosition.x + 1 && enPassantTarget.y == _boardPosition.y + direction) ||
                    (enPassantTarget.x == _boardPosition.x - 1 && enPassantTarget.y == _boardPosition.y + direction))
                {
                    moves.Add(enPassantTarget);
                }
            }
        }

        return moves;
    }
}