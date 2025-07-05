using UnityEngine;

public static class MoveConverter
{
    private static readonly string _fileChars = "abcdefgh";

    public static string ToDescriptiveNotation(ChessPiece piece, Vector2Int to)
    {
        string pieceName = piece.Type.ToString();

        string destinationSquare = _fileChars[to.x] + (to.y + 1).ToString();

        return $"{pieceName} {destinationSquare}";
    }


    private static readonly string _pieceChars = " KQRBNP";

    public static string ToStandardNotation(ChessPiece piece, Vector2Int to, bool isCapture, bool isCheck, bool isCheckmate)
    {
        if (piece.Type == PieceType.King && Mathf.Abs(to.x - piece._boardPosition.x) == 2)
        {
            return to.x > piece._boardPosition.x ? "O-O" : "O-O-O";
        }

        string notation = "";

        if (piece.Type != PieceType.Pawn)
        {
            notation += _pieceChars[(int)piece.Type + 1];
        }

        if (isCapture)
        {
            if (piece.Type == PieceType.Pawn)
            {
                notation += _fileChars[piece._boardPosition.x];
            }
            notation += "x";
        }

        notation += _fileChars[to.x];
        notation += (to.y + 1).ToString();

        if (isCheckmate)
        {
            notation += "#";
        }
        else if (isCheck)
        {
            notation += "+";
        }

        return notation;
    }
}