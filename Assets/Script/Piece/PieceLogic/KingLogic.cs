using System.Collections.Generic;
using UnityEngine;

public class KingLogic : PieceLogic
{
    public KingLogic() { this.Type = PieceType.King; }

    public override List<Vector2Int> GetPossibleMoves(BoardState boardState)
    {
        var moves = new List<Vector2Int>();

        // --- 1. Standard King Moves ---
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

        // --- 2. Castling ---
        if (!this.HasMoved && CanCastle(boardState, true))
        {
            moves.Add(new Vector2Int(this.Position.x + 2, this.Position.y));
        }
        if (!this.HasMoved && CanCastle(boardState, false))
        {
            moves.Add(new Vector2Int(this.Position.x - 2, this.Position.y));
        }

        // --- 3. POWERED-UP MOVE LOGIC HAS BEEN REMOVED ---

        return moves;
    }

    public override List<Vector2Int> GetAttackMoves(BoardState boardState)
    {
        var moves = new List<Vector2Int>();
        // A king always attacks the 8 squares around it.
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;
                Vector2Int nextPos = this.Position + new Vector2Int(x, y);
                if (IsOnBoard(nextPos))
                {
                    moves.Add(nextPos);
                }
            }
        }
        return moves;
    }

    // --- CanCastle logic is unchanged and correct ---
    private bool CanCastle(BoardState boardState, bool isKingside)
    {
        if (this.HasMoved) return false;
        int startRank = this.IsWhite ? 0 : 7;
        if (this.Position != new Vector2Int(4, startRank)) return false;
        if (MoveValidator.Instance.IsInCheck(this.IsWhite, boardState)) return false;

        if (isKingside)
        {
            var rookData = boardState.Pieces[7, startRank];
            if (rookData == null || rookData.Value.Type != PieceType.Rook || rookData.Value.HasMoved) return false;
            if (boardState.Pieces[5, startRank] != null || boardState.Pieces[6, startRank] != null) return false;
            if (MoveValidator.Instance.IsSquareAttacked(new Vector2Int(5, startRank), !this.IsWhite, boardState) ||
                MoveValidator.Instance.IsSquareAttacked(new Vector2Int(6, startRank), !this.IsWhite, boardState)) return false;
        }
        else // Queenside
        {
            var rookData = boardState.Pieces[0, startRank];
            if (rookData == null || rookData.Value.Type != PieceType.Rook || rookData.Value.HasMoved) return false;
            if (boardState.Pieces[1, startRank] != null || boardState.Pieces[2, startRank] != null || boardState.Pieces[3, startRank] != null) return false;
            if (MoveValidator.Instance.IsSquareAttacked(new Vector2Int(2, startRank), !this.IsWhite, boardState) ||
                MoveValidator.Instance.IsSquareAttacked(new Vector2Int(3, startRank), !this.IsWhite, boardState)) return false;
        }
        return true;
    }
}