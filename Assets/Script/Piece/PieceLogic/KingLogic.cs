using System.Collections.Generic;
using UnityEngine;

public class KingLogic : PieceLogic
{
    public KingLogic() { this.Type = PieceType.King; }
    public override List<Vector2Int> GetAttackMoves(BoardState boardState)
    {
        var moves = new List<Vector2Int>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;
                Vector2Int nextPos = this.Position + new Vector2Int(x, y);
                if (IsOnBoard(nextPos)) moves.Add(nextPos);
            }
        }
        return moves;
    }
    public override List<Vector2Int> GetPossibleMoves(BoardState boardState)
    {
        var moves = GetAttackMoves(boardState);
        moves.RemoveAll(move => {
            var piece = boardState.Pieces[move.x, move.y];
            return piece != null && piece.Value.IsWhite == this.IsWhite;
        });
        return moves;
    }
}