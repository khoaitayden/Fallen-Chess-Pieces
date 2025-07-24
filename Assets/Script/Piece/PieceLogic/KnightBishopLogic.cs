using System.Collections.Generic;
using UnityEngine;

public class KnightBishopLogic : PieceLogic
{
    public override List<Vector2Int> GetPossibleMoves(BoardState boardState)
    {
        var moves = new List<Vector2Int>();

        // 1. Get Knight moves.
        var knightLogic = new KnightLogic();
        knightLogic.Initialize(this.IsWhite, this.Position, this.HasMoved, PieceType.Knight);
        moves.AddRange(knightLogic.GetPossibleMoves(boardState));

        // 2. Get Bishop moves.
        var bishopLogic = new BishopLogic();
        bishopLogic.Initialize(this.IsWhite, this.Position, this.HasMoved, PieceType.Bishop);
        moves.AddRange(bishopLogic.GetPossibleMoves(boardState));
        
        return moves;
    }
    
    public override List<Vector2Int> GetAttackMoves(BoardState boardState) => GetPossibleMoves(boardState);
}