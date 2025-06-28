using System.Collections.Generic;
using UnityEngine;

public class PawnPiece : ChessPiece
{
    public override List<Vector2Int> GetAttackMoves(Chessboard board)
    {
        var moves = new List<Vector2Int>();
        int direction = IsWhite ? 1 : -1;

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

        //Forward Move
        Vector2Int forwardMove = new Vector2Int(_boardPosition.x, _boardPosition.y + direction);
        if (board.GetSquareAt(forwardMove) != null && board.GetPieceAt(forwardMove) == null)
        {
            moves.Add(forwardMove);

            //Double Forward Move
            if (!_hasMoved)
            {
                Vector2Int doubleForwardMove = new Vector2Int(_boardPosition.x, _boardPosition.y + 2 * direction);
                if (board.GetSquareAt(doubleForwardMove) != null && board.GetPieceAt(doubleForwardMove) == null)
                {
                    moves.Add(doubleForwardMove);
                }
            }
        }

        //Diagonal Capture Right
        Vector2Int captureRight = new Vector2Int(_boardPosition.x + 1, _boardPosition.y + direction);
        if (board.GetSquareAt(captureRight) != null)
        {
            ChessPiece pieceRight = board.GetPieceAt(captureRight);
            if (pieceRight != null && pieceRight.IsWhite != this.IsWhite)
            {
                moves.Add(captureRight);
            }
        }

        //Diagonal Capture Left
        Vector2Int captureLeft = new Vector2Int(_boardPosition.x - 1, _boardPosition.y + direction);
        if (board.GetSquareAt(captureLeft) != null)
        {
            ChessPiece pieceLeft = board.GetPieceAt(captureLeft);
            if (pieceLeft != null && pieceLeft.IsWhite != this.IsWhite)
            {
                moves.Add(captureLeft);
            }
        }

        //En Passant
        Vector2Int enPassantTarget = TurnManager.Instance.EnPassantTargetSquare;
        if (enPassantTarget != new Vector2Int(-1, -1))
        {
            if ((_boardPosition.y == 4 && IsWhite) || (_boardPosition.y == 3 && !IsWhite))
            {
                if ((enPassantTarget.x == _boardPosition.x + 1 && enPassantTarget.y == _boardPosition.y + direction) ||
                    (enPassantTarget.x == _boardPosition.x - 1 && enPassantTarget.y == _boardPosition.y + direction))
                {
                    if (board.GetSquareAt(enPassantTarget) != null)
                    {
                        moves.Add(enPassantTarget);
                    }
                }
            }
        }

        return moves;
    }
}