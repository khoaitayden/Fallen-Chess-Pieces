using System.Collections.Generic;
using UnityEngine;

public class RookPiece : ChessPiece
{
    public override List<Vector2Int> GetPossibleMoves(Chessboard board)
    {
        var moves = new List<Vector2Int>();

        CheckDirection(moves, board, new Vector2Int(0, 1)); 
        CheckDirection(moves, board, new Vector2Int(0, -1));
        CheckDirection(moves, board, new Vector2Int(-1, 0));
        CheckDirection(moves, board, new Vector2Int(1, 0));

        return moves;
    }

    private void CheckDirection(List<Vector2Int> moves, Chessboard board, Vector2Int direction)
    {
        Vector2Int nextPos = _boardPosition + direction;

        while (board.GetSquareAt(nextPos) != null) 
        {
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