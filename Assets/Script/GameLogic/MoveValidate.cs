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

                    List<Vector2Int> moves = piece.GetAttackMoves(chessboard);
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
    public bool CanCastleKingside(ChessPiece king)
    {
        // 1. King and Rook must not have moved
        if (king.Type != PieceType.King || king._hasMoved) return false;
        
        ChessPiece rook = chessboard.GetPieceAt(new Vector2Int(7, king._boardPosition.y));
        if (rook == null || rook.Type != PieceType.Rook || rook._hasMoved) return false;

        // 2. No pieces between King and Rook
        if (chessboard.GetPieceAt(new Vector2Int(5, king._boardPosition.y)) != null) return false;
        if (chessboard.GetPieceAt(new Vector2Int(6, king._boardPosition.y)) != null) return false;

        // 3. King must not be in check, and must not pass through or land on a square attacked by the enemy
        if (IsInCheck(king.IsWhite)) return false;
        if (IsSquareAttacked(new Vector2Int(5, king._boardPosition.y), !king.IsWhite)) return false;
        if (IsSquareAttacked(new Vector2Int(6, king._boardPosition.y), !king.IsWhite)) return false;

        return true;
    }

    public bool CanCastleQueenside(ChessPiece king)
    {
        // 1. King and Rook must not have moved
        if (king.Type != PieceType.King || king._hasMoved) return false;

        ChessPiece rook = chessboard.GetPieceAt(new Vector2Int(0, king._boardPosition.y));
        if (rook == null || rook.Type != PieceType.Rook || rook._hasMoved) return false;

        // 2. No pieces between King and Rook.
        if (chessboard.GetPieceAt(new Vector2Int(1, king._boardPosition.y)) != null) return false;
        if (chessboard.GetPieceAt(new Vector2Int(2, king._boardPosition.y)) != null) return false;
        if (chessboard.GetPieceAt(new Vector2Int(3, king._boardPosition.y)) != null) return false;

        // 3. King must not be in check, and must not pass through or land on a square attacked by the enemy.
        if (IsInCheck(king.IsWhite)) return false;
        if (IsSquareAttacked(new Vector2Int(2, king._boardPosition.y), !king.IsWhite)) return false;
        if (IsSquareAttacked(new Vector2Int(3, king._boardPosition.y), !king.IsWhite)) return false;

        return true;
    }

    // Checks if a specific square is under attack by the opponent
    private bool IsSquareAttacked(Vector2Int square, bool byWhitePlayer)
    {
        for (int x = 0; x < Constants.BOARD_SIZE; x++)
        {
            for (int y = 0; y < Constants.BOARD_SIZE; y++)
            {
                ChessPiece piece = chessboard.GetPieceAt(new Vector2Int(x, y));
                if (piece != null && piece.IsWhite == byWhitePlayer)
                {
                    if (piece.GetAttackMoves(chessboard).Contains(square))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}