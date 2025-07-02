using UnityEngine;

public static class MoveConverter
{
    // Helper arrays for converting coordinates.
    private static readonly string _fileChars = "abcdefgh";

    // --- NEW METHOD FOR DESCRIPTIVE NOTATION ---
    public static string ToDescriptiveNotation(ChessPiece piece, Vector2Int to)
    {
        // Get the full name of the piece type.
        // The .ToString() method on an enum gives you its name as a string.
        string pieceName = piece.Type.ToString();

        // Get the destination square in algebraic notation (e.g., "e4").
        string destinationSquare = _fileChars[to.x] + (to.y + 1).ToString();

        // Combine them into the desired format.
        return $"{pieceName} {destinationSquare}";
    }


    // --- YOUR EXISTING METHOD FOR STANDARD NOTATION (Keep it for future use) ---
    private static readonly string _pieceChars = " KQRBNP"; // Note the space at the start

    public static string ToStandardNotation(ChessPiece piece, Vector2Int to, bool isCapture, bool isCheck, bool isCheckmate)
    {
        // Handle special case for castling
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