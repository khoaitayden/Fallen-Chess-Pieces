using System.Collections.Generic;
using UnityEngine;

public class KnightBishopRookLogic : PieceLogic
{
    public override List<Vector2Int> GetPossibleMoves(BoardState boardState)
    {
        var moves = new List<Vector2Int>();

        // 1. Get Knight moves.
        var knightLogic = new KnightLogic();
        knightLogic.Initialize(this.IsWhite, this.Position, this.HasMoved, PieceType.Knight);
        moves.AddRange(knightLogic.GetPossibleMoves(boardState));

        // 2. Get Queen moves (which is Rook + Bishop).
        var queenLogic = new QueenLogic();
        queenLogic.Initialize(this.IsWhite, this.Position, this.HasMoved, PieceType.Queen);
        moves.AddRange(queenLogic.GetPossibleMoves(boardState));
        
        return moves;
    }
    
    public override List<Vector2Int> GetAttackMoves(BoardState boardState) => GetPossibleMoves(boardState);
}