using System.Collections.Generic;
using UnityEngine;

public class KnightRookLogic : PieceLogic
{
    // Note: We don't set a 'Type' in the constructor for combination pieces.

    public override List<Vector2Int> GetPossibleMoves(BoardState boardState)
    {
        var moves = new List<Vector2Int>();

        // 1. Get all of a Knight's possible moves from this position.
        var knightLogic = new KnightLogic();
        knightLogic.Initialize(this.IsWhite, this.Position, this.HasMoved, PieceType.Knight);
        moves.AddRange(knightLogic.GetPossibleMoves(boardState));

        // 2. Get all of a Rook's possible moves from this position.
        var rookLogic = new RookLogic();
        rookLogic.Initialize(this.IsWhite, this.Position, this.HasMoved, PieceType.Rook);
        moves.AddRange(rookLogic.GetPossibleMoves(boardState));
        
        return moves;
    }

    // The squares it attacks are the same as the squares it can move to.
    public override List<Vector2Int> GetAttackMoves(BoardState boardState) => GetPossibleMoves(boardState);
}