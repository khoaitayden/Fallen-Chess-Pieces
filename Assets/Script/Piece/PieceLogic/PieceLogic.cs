// In PieceLogic.cs
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