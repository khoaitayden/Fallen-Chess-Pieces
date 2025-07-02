using UnityEngine;

// A struct is a lightweight data container. Perfect for this.
public struct MoveData
{
    public readonly PieceType Piece;
    public readonly Vector2Int From;
    public readonly Vector2Int To;
    public readonly string Notation;

    // The constructor makes it easy to create a new Move object.
    public MoveData(PieceType piece, Vector2Int from, Vector2Int to, string notation)
    {
        Piece = piece;
        From = from;
        To = to;
        Notation = notation;
    }
}