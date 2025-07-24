using System.Collections.Generic;
using UnityEngine;

public abstract class PieceLogic
{
    public bool IsWhite { get; protected set; }
    public Vector2Int Position { get; protected set; }
    public bool HasMoved { get; protected set; }
    public PieceType Type { get; protected set; }

    public virtual void Initialize(bool isWhite, Vector2Int position, bool hasMoved, PieceType type)
    {
        this.IsWhite = isWhite;
        this.Position = position;
        this.HasMoved = hasMoved;
        this.Type = type;
    }
    
    public abstract List<Vector2Int> GetPossibleMoves(BoardState boardState);
    public abstract List<Vector2Int> GetAttackMoves(BoardState boardState);

    // --- NEW HELPER METHOD FOR INHERITED POWERS ---
    protected void AddInheritedMoves(List<Vector2Int> moves, BoardState boardState)
    {
        List<PieceType> inheritedPowers = PowerManager.Instance.GetPowersForPiece(this.Position);
        if (inheritedPowers.Count == 0) return;

        // --- THIS IS THE FIX ---
        // We check against 'this.Type', which is the type of the current logic object.
        if (inheritedPowers.Contains(PieceType.Rook))
        {
            if (this.Type != PieceType.Rook)
            {
                Vector2Int[] directions = { new(0, 1), new(0, -1), new(1, 0), new(-1, 0) };
                foreach (var dir in directions) CheckSlidingDirection(moves, boardState, dir);
            }
        }
        if (inheritedPowers.Contains(PieceType.Bishop))
        {
            if (this.Type != PieceType.Bishop)
            {
                Vector2Int[] directions = { new(1, 1), new(1, -1), new(-1, 1), new(-1, -1) };
                foreach (var dir in directions) CheckSlidingDirection(moves, boardState, dir);
            }
        }
        if (inheritedPowers.Contains(PieceType.Knight))
        {
            if (this.Type != PieceType.Knight)
            {
                Vector2Int[] offsets = { new(1, 2), new(1, -2), new(-1, 2), new(-1, -2), new(2, 1), new(2, -1), new(-2, 1), new(-2, -1) };
                foreach (var offset in offsets)
                {
                    Vector2Int nextPos = this.Position + offset;
                    if (IsOnBoard(nextPos))
                    {
                        var targetPiece = boardState.Pieces[nextPos.x, nextPos.y];
                        if (targetPiece == null || targetPiece.Value.IsWhite != this.IsWhite) moves.Add(nextPos);
                    }
                }
            }
        }
        // -----------------------
    }
    // ---------------------------------------------

    protected bool IsOnBoard(Vector2Int pos) => pos.x >= 0 && pos.x < 8 && pos.y >= 0 && pos.y < 8;
    protected void CheckSlidingDirection(List<Vector2Int> moves, BoardState state, Vector2Int direction)
    {
        Vector2Int nextPos = this.Position + direction;
        while (IsOnBoard(nextPos))
        {
            var targetPiece = state.Pieces[nextPos.x, nextPos.y];
            if (targetPiece == null)
            {
                moves.Add(nextPos);
                nextPos += direction;
            }
            else
            {
                if (targetPiece.Value.IsWhite != this.IsWhite)
                {
                    moves.Add(nextPos);
                }
                break;
            }
        }
    }
}