using System.Collections.Generic;
using UnityEngine;

public class PawnLogic : PieceLogic
{
    public PawnLogic() { this.Type = PieceType.Pawn; }

    public override List<Vector2Int> GetAttackMoves(BoardState boardState)
    {
        var moves = new List<Vector2Int>();
        int direction = this.IsWhite ? 1 : -1;
        Vector2Int[] attackPositions = { new(this.Position.x + 1, this.Position.y + direction), new(this.Position.x - 1, this.Position.y + direction) };
        foreach (var pos in attackPositions) if (IsOnBoard(pos)) moves.Add(pos);
        return moves;
    }

    public override List<Vector2Int> GetPossibleMoves(BoardState boardState)
    {
        var moves = new List<Vector2Int>();
        int direction = this.IsWhite ? 1 : -1;
        Vector2Int oneForward = new(this.Position.x, this.Position.y + direction);
        if (IsOnBoard(oneForward) && boardState.Pieces[oneForward.x, oneForward.y] == null)
        {
            moves.Add(oneForward);
            if (!this.HasMoved)
            {
                Vector2Int twoForward = new(this.Position.x, this.Position.y + 2 * direction);
                if (IsOnBoard(twoForward) && boardState.Pieces[twoForward.x, twoForward.y] == null) moves.Add(twoForward);
            }
        }
        Vector2Int[] capturePositions = { new(this.Position.x + 1, this.Position.y + direction), new(this.Position.x - 1, this.Position.y + direction) };
        foreach (var pos in capturePositions)
        {
            if (IsOnBoard(pos))
            {
                var pieceAtTarget = boardState.Pieces[pos.x, pos.y];
                if (pieceAtTarget != null && pieceAtTarget.Value.IsWhite != this.IsWhite) moves.Add(pos);
            }
        }
        Vector2Int enPassantTarget = boardState.EnPassantTargetSquare;
        if (enPassantTarget != new Vector2Int(-1, -1) && enPassantTarget.y == this.Position.y + direction && Mathf.Abs(enPassantTarget.x - this.Position.x) == 1)
        {
            moves.Add(enPassantTarget);
        }
        return moves;
    }
}