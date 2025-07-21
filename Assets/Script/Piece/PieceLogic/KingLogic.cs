using System.Collections.Generic;
using UnityEngine;

public class KingLogic : PieceLogic
{
    public KingLogic() { this.Type = PieceType.King; }

    public override List<Vector2Int> GetAttackMoves(BoardState boardState)
    {
        var moves = new List<Vector2Int>();

        // --- 1. Standard King Attacks ---
        // A king always attacks the 8 squares around it, regardless of powers.
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

        // --- 2. Powered-Up Attacks ---
        // If the king has powers, it ALSO attacks like those pieces.
        if (KingPowerManager.Instance.KingHasPower(this.IsWhite, PieceType.Knight))
        {
            // We need the Knight's ATTACK moves, not its possible moves.
            var knightLogic = new KnightLogic();
            knightLogic.Initialize(this.IsWhite, this.Position, true, PieceType.Knight);
            moves.AddRange(knightLogic.GetAttackMoves(boardState));
        }
        if (KingPowerManager.Instance.KingHasPower(this.IsWhite, PieceType.Rook))
        {
            var rookLogic = new RookLogic();
            rookLogic.Initialize(this.IsWhite, this.Position, true, PieceType.Rook);
            moves.AddRange(rookLogic.GetAttackMoves(boardState));
        }
        if (KingPowerManager.Instance.KingHasPower(this.IsWhite, PieceType.Bishop))
        {
            var bishopLogic = new BishopLogic();
            bishopLogic.Initialize(this.IsWhite, this.Position, true, PieceType.Bishop);
            moves.AddRange(bishopLogic.GetAttackMoves(boardState));
        }

        return moves;
    }

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

        // --- 2. Castling (Unchanged) ---
        if (!this.HasMoved && CanCastle(boardState, true))
        {
            moves.Add(new Vector2Int(this.Position.x + 2, this.Position.y));
        }
        if (!this.HasMoved && CanCastle(boardState, false))
        {
            moves.Add(new Vector2Int(this.Position.x - 2, this.Position.y));
        }

        // --- 3. NEW: Direct Power Calculation ---
        if (KingPowerManager.Instance.KingHasPower(this.IsWhite, PieceType.Knight))
        {
            Vector2Int[] knightMoves = {
                new(1, 2), new(1, -2), new(-1, 2), new(-1, -2),
                new(2, 1), new(2, -1), new(-2, 1), new(-2, -1)
            };
            foreach (var move in knightMoves)
            {
                Vector2Int nextPos = this.Position + move;
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
        if (KingPowerManager.Instance.KingHasPower(this.IsWhite, PieceType.Rook))
        {
            Vector2Int[] directions = { new(0, 1), new(0, -1), new(1, 0), new(-1, 0) };
            foreach (var dir in directions) CheckSlidingDirection(moves, boardState, dir);
        }
        if (KingPowerManager.Instance.KingHasPower(this.IsWhite, PieceType.Bishop))
        {
            Vector2Int[] directions = { new(1, 1), new(1, -1), new(-1, 1), new(-1, -1) };
            foreach (var dir in directions) CheckSlidingDirection(moves, boardState, dir);
        }

        return moves;
    }

    private bool CanCastle(BoardState boardState, bool isKingside)
    {
        // --- THIS IS THE ROBUST FIX ---
        // 1. The king must not have moved.
        if (this.HasMoved) return false;

        // 2. The king must be on its starting square.
        int startRank = this.IsWhite ? 0 : 7;
        if (this.Position != new Vector2Int(4, startRank)) return false;
        // ---------------------------------

        if (MoveValidator.Instance.IsInCheck(this.IsWhite, boardState)) return false;

        if (isKingside)
        {
            // The rook must also be on its starting square.
            var rookData = boardState.Pieces[7, startRank];
            if (rookData == null || rookData.Value.Type != PieceType.Rook || rookData.Value.HasMoved) return false;

            // Check for pieces in between.
            if (boardState.Pieces[5, startRank] != null || boardState.Pieces[6, startRank] != null) return false;

            // Check if the squares the king moves through are under attack.
            if (MoveValidator.Instance.IsSquareAttacked(new Vector2Int(5, startRank), !this.IsWhite, boardState) ||
                MoveValidator.Instance.IsSquareAttacked(new Vector2Int(6, startRank), !this.IsWhite, boardState)) return false;
        }
        else // Queenside
        {
            // The rook must also be on its starting square.
            var rookData = boardState.Pieces[0, startRank];
            if (rookData == null || rookData.Value.Type != PieceType.Rook || rookData.Value.HasMoved) return false;

            // Check for pieces in between.
            if (boardState.Pieces[1, startRank] != null || boardState.Pieces[2, startRank] != null || boardState.Pieces[3, startRank] != null) return false;

            // Check if the squares the king moves through are under attack.
            if (MoveValidator.Instance.IsSquareAttacked(new Vector2Int(2, startRank), !this.IsWhite, boardState) ||
                MoveValidator.Instance.IsSquareAttacked(new Vector2Int(3, startRank), !this.IsWhite, boardState)) return false;
        }

        return true;
    }
}