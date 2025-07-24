using System.Collections.Generic;
using UnityEngine;

public class BishopLogic : PieceLogic
{
    public BishopLogic() { this.Type = PieceType.Bishop; }
    public override List<Vector2Int> GetAttackMoves(BoardState boardState) => GetPossibleMoves(boardState);
    public override List<Vector2Int> GetPossibleMoves(BoardState boardState)
    {
        // 1. Get my native Bishop moves.
        var moves = new List<Vector2Int>();
        Vector2Int[] directions = { new(1, 1), new(1, -1), new(-1, 1), new(-1, -1) };
        foreach (var dir in directions) CheckSlidingDirection(moves, boardState, dir);
        
        // 2. Add any inherited powers (e.g., from a Rook or Knight).
        AddInheritedMoves(moves, boardState);

        return moves;
    }
}