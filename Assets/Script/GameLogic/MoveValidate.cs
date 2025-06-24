// Create new script: MoveValidator.cs
using UnityEngine;
using System.Collections.Generic;

public class MoveValidator : MonoBehaviour
{
    public static MoveValidator Instance { get; private set; }

    [SerializeField] private Chessboard chessboard;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public List<Vector2Int> GetValidMoves(ChessPiece piece)
    {
        List<Vector2Int> validMoves = new List<Vector2Int>();
        List<Vector2Int> possibleMoves = piece.GetPossibleMoves(chessboard);

        Vector2Int originalPosition = piece._boardPosition;
        //Check king safety 
        foreach (Vector2Int move in possibleMoves)
        {
            ChessPiece capturedPiece = chessboard.SimulateMove(piece, move);

            if (!IsInCheck(piece.IsWhite))
            {
                validMoves.Add(move);
            }

            chessboard.UndoSimulatedMove(piece, originalPosition, capturedPiece);
        }

        return validMoves;
    }


    // Checks if King check
    public bool IsInCheck(bool isWhitePlayer)
    {
        Vector2Int kingPosition = FindKingPosition(isWhitePlayer);
        if (kingPosition == new Vector2Int(-1, -1)) return false;

        // Check if any enemy piece can attack the king's square
        for (int x = 0; x < Constants.BOARD_SIZE; x++)
        {
            for (int y = 0; y < Constants.BOARD_SIZE; y++)
            {
                ChessPiece piece = chessboard.GetPieceAt(new Vector2Int(x, y));
                if (piece != null && piece.IsWhite != isWhitePlayer)
                {
                    List<Vector2Int> moves = piece.GetPossibleMoves(chessboard);
                    if (moves.Contains(kingPosition))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private Vector2Int FindKingPosition(bool isWhitePlayer)
    {
        for (int x = 0; x < Constants.BOARD_SIZE; x++)
        {
            for (int y = 0; y < Constants.BOARD_SIZE; y++)
            {
                ChessPiece piece = chessboard.GetPieceAt(new Vector2Int(x, y));
                if (piece != null && piece.IsWhite == isWhitePlayer && piece.Type == PieceType.King)
                {
                    return piece._boardPosition;
                }
            }
        }
        return new Vector2Int(-1, -1);
    }
    public bool IsCheckmate(bool isWhitePlayer)
    {
        if (!IsInCheck(isWhitePlayer))
        {
            return false;
        }

        return !HasAnyValidMoves(isWhitePlayer);
    }

    public bool IsStalemate(bool isWhitePlayer)
    {
        if (IsInCheck(isWhitePlayer))
        {
            return false;
        }

        return !HasAnyValidMoves(isWhitePlayer);
    }

    private bool HasAnyValidMoves(bool isWhitePlayer)
    {
        for (int x = 0; x < Constants.BOARD_SIZE; x++)
        {
            for (int y = 0; y < Constants.BOARD_SIZE; y++)
            {
                ChessPiece piece = chessboard.GetPieceAt(new Vector2Int(x, y));

                if (piece != null && piece.IsWhite == isWhitePlayer)
                {
                    if (GetValidMoves(piece).Count > 0)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}