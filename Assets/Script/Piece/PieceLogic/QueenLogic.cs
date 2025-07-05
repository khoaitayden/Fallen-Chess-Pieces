using System.Collections.Generic;
using UnityEngine;

public class QueenLogic : PieceLogic
{
    public QueenLogic() { this.Type = PieceType.Queen; }
    public override List<Vector2Int> GetAttackMoves(BoardState boardState) => GetPossibleMoves(boardState);
    public override List<Vector2Int> GetPossibleMoves(BoardState boardState)
    {
        var moves = new List<Vector2Int>();
        Vector2Int[] directions = { new(0, 1), new(0, -1), new(1, 0), new(-1, 0), new(1, 1), new(1, -1), new(-1, 1), new(-1, -1) };
        foreach (var dir in directions) CheckSlidingDirection(moves, boardState, dir);
        return moves;
    }
}