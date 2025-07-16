using System.Collections.Generic;
using UnityEngine;

public class KingLogic : PieceLogic
{
    public KingLogic() { this.Type = PieceType.King; }

    public override List<Vector2Int> GetAttackMoves(BoardState boardState)
    {
        var moves = new List<Vector2Int>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;
                Vector2Int nextPos = this.Position + new Vector2Int(x, y);
                if (IsOnBoard(nextPos)) moves.Add(nextPos);
            }
        }
        return moves;
    }

    public override List<Vector2Int> GetPossibleMoves(BoardState boardState)
    {
        var moves = new List<Vector2Int>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;
                Vector2Int nextPos = this.Position + new Vector2Int(x, y);
                if (IsOnBoard(nextPos))
                {
                    var piece = boardState.Pieces[nextPos.x, nextPos.y];
                    if (piece == null || piece.Value.IsWhite != this.IsWhite)
                    {
                        moves.Add(nextPos);
                    }
                }
            }
        }

        if (!this.HasMoved)
        {
            if (CanCastle(boardState, true))
            {
                moves.Add(new Vector2Int(this.Position.x + 2, this.Position.y));
            }
            if (CanCastle(boardState, false))
            {
                moves.Add(new Vector2Int(this.Position.x - 2, this.Position.y));
            }
        }
        return moves;
    }

    private bool CanCastle(BoardState boardState, bool isKingside)
    {
        if (this.HasMoved) return false;
        if (MoveValidator.Instance.IsInCheck(this.IsWhite, boardState)) return false;

        if (isKingside)
        {
            var rookData = boardState.Pieces[7, this.Position.y];
            if (rookData == null || rookData.Value.Type != PieceType.Rook || rookData.Value.HasMoved) return false;

            if (boardState.Pieces[5, this.Position.y] != null || boardState.Pieces[6, this.Position.y] != null) return false;

            if (MoveValidator.Instance.IsSquareAttacked(new Vector2Int(5, this.Position.y), !this.IsWhite, boardState) ||
                MoveValidator.Instance.IsSquareAttacked(new Vector2Int(6, this.Position.y), !this.IsWhite, boardState)) return false;
        }
        else 
        {
            var rookData = boardState.Pieces[0, this.Position.y];
            if (rookData == null || rookData.Value.Type != PieceType.Rook || rookData.Value.HasMoved) return false;

            if (boardState.Pieces[1, this.Position.y] != null || boardState.Pieces[2, this.Position.y] != null || boardState.Pieces[3, this.Position.y] != null) return false;

            if (MoveValidator.Instance.IsSquareAttacked(new Vector2Int(2, this.Position.y), !this.IsWhite, boardState) ||
                MoveValidator.Instance.IsSquareAttacked(new Vector2Int(3, this.Position.y), !this.IsWhite, boardState)) return false;
        }

        return true;
    }
}