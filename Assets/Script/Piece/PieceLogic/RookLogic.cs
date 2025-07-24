using System.Collections.Generic;
using UnityEngine;

public class RookLogic : PieceLogic
{
    public RookLogic() { this.Type = PieceType.Rook; }
    public override List<Vector2Int> GetAttackMoves(BoardState boardState) => GetPossibleMoves(boardState);
    public override List<Vector2Int> GetPossibleMoves(BoardState boardState)
    {
        // 1. Get my native Rook moves.
        var moves = new List<Vector2Int>();
        Vector2Int[] directions = { new(0, 1), new(0, -1), new(1, 0), new(-1, 0) };
        foreach (var dir in directions) CheckSlidingDirection(moves, boardState, dir);

        // 2. Add any inherited powers (e.g., from a Knight).
        AddInheritedMoves(moves, boardState);

        return moves;
    }
}