// In PawnPiece.cs
using System.Collections.Generic;
using UnityEngine;

public class PawnPiece : ChessPiece
{
    private bool IsOnBoard(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < Constants.BOARD_SIZE && pos.y >= 0 && pos.y < Constants.BOARD_SIZE;
    }

    public override List<Vector2Int> GetAttackMoves(BoardState boardState)
    {
        var moves = new List<Vector2Int>();
        int direction = IsWhite ? 1 : -1;

        Vector2Int[] attackPositions = 
        {
            new Vector2Int(_boardPosition.x + 1, _boardPosition.y + direction),
            new Vector2Int(_boardPosition.x - 1, _boardPosition.y + direction)
        };

        foreach (var pos in attackPositions)
        {
            if (IsOnBoard(pos))
            {
                moves.Add(pos);
            }
        }
        return moves;
    }

    public override List<Vector2Int> GetPossibleMoves(BoardState boardState)
    {
        var moves = new List<Vector2Int>();
        int direction = IsWhite ? 1 : -1;

        // --- 1. Forward Move ---
        Vector2Int oneForward = new Vector2Int(_boardPosition.x, _boardPosition.y + direction);
        // Check if the square is on the board AND if it's empty.
        if (IsOnBoard(oneForward) && boardState.Pieces[oneForward.x, oneForward.y] == null)
        {
            moves.Add(oneForward);

            // --- 2. Double Forward Move (only on first move) ---
            if (!_hasMoved)
            {
                Vector2Int twoForward = new Vector2Int(_boardPosition.x, _boardPosition.y + 2 * direction);
                // Check if this square is also on the board and empty.
                if (IsOnBoard(twoForward) && boardState.Pieces[twoForward.x, twoForward.y] == null)
                {
                    moves.Add(twoForward);
                }
            }
        }

        // --- 3. Diagonal Captures ---
        Vector2Int[] capturePositions = 
        {
            new Vector2Int(_boardPosition.x + 1, _boardPosition.y + direction),
            new Vector2Int(_boardPosition.x - 1, _boardPosition.y + direction)
        };

        foreach (var pos in capturePositions)
        {
            if (IsOnBoard(pos))
            {
                // Get the piece data directly from the array.
                var pieceAtTarget = boardState.Pieces[pos.x, pos.y];
                // Check if there is a piece AND if it's an enemy piece.
                if (pieceAtTarget != null && pieceAtTarget.Value.IsWhite != this.IsWhite)
                {
                    moves.Add(pos);
                }
            }
        }

        // --- 4. En Passant ---
        // The BoardState now correctly holds the en passant target square.
        Vector2Int enPassantTarget = boardState.EnPassantTargetSquare;
        if (enPassantTarget != new Vector2Int(-1, -1))
        {
            // Check if the en passant target is one of the diagonal attack squares.
            if (enPassantTarget.y == _boardPosition.y + direction && 
               (enPassantTarget.x == _boardPosition.x + 1 || enPassantTarget.x == _boardPosition.x - 1))
            {
                moves.Add(enPassantTarget);
            }
        }

        return moves;
    }
}