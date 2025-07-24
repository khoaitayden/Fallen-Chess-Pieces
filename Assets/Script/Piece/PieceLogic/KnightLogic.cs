using System.Collections.Generic;
using UnityEngine;

public class KnightLogic : PieceLogic
{
    public KnightLogic() { this.Type = PieceType.Knight; }
    public override List<Vector2Int> GetAttackMoves(BoardState boardState) => GetPossibleMoves(boardState);
    public override List<Vector2Int> GetPossibleMoves(BoardState boardState)
    {
        // 1. Get my native Knight moves.
        var moves = new List<Vector2Int>();
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

        // 2. Add any inherited powers (e.g., from a Rook or Bishop).
        AddInheritedMoves(moves, boardState);
        
        return moves;
    }
}