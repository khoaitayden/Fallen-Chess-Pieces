using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MoveValidator : MonoBehaviour
{
    public static MoveValidator Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private PieceLogic GetPieceLogic(BoardState.PieceData pieceData, Vector2Int position)
    {
        PieceLogic logic;
        switch (pieceData.Type)
        {
            case PieceType.Pawn:   logic = new PawnLogic();   break;
            case PieceType.Rook:   logic = new RookLogic();   break;
            case PieceType.Knight: logic = new KnightLogic(); break;
            case PieceType.Bishop: logic = new BishopLogic(); break;
            case PieceType.Queen:  logic = new QueenLogic();  break;
            case PieceType.King:   logic = new KingLogic();   break;
            default: return null;
        }
        logic.Initialize(pieceData.IsWhite, position, pieceData.HasMoved, pieceData.Type);
        return logic;
    }

    public List<Vector2Int> GetValidMoves(ChessPiece piece) => GetValidMoves(piece._boardPosition, Chessboard.Instance.CreateBoardState());
    public bool IsInCheck(bool isWhitePlayer) => IsInCheck(isWhitePlayer, Chessboard.Instance.CreateBoardState());
    public bool IsCheckmate(bool isWhitePlayer) => IsCheckmate(isWhitePlayer, Chessboard.Instance.CreateBoardState());
    public bool IsStalemate(bool isWhitePlayer) => IsStalemate(isWhitePlayer, Chessboard.Instance.CreateBoardState());
    public bool HasInsufficientMaterial() => HasInsufficientMaterial(Chessboard.Instance.CreateBoardState());

    public List<Vector2Int> GetValidMoves(Vector2Int piecePosition, BoardState boardState)
    {
        var validMoves = new List<Vector2Int>();
        var pieceData = boardState.Pieces[piecePosition.x, piecePosition.y];
        if (pieceData == null) return validMoves;
        PieceLogic pieceLogic = GetPieceLogic(pieceData.Value, piecePosition);
        if (pieceLogic == null) return validMoves;
        var possibleMoves = pieceLogic.GetPossibleMoves(boardState);
        foreach (var move in possibleMoves)
        {
            BoardState newState = new BoardState(boardState);
            SimulateMoveOnState(newState, new MoveData(pieceData.Value.Type, piecePosition, move, ""));
            if (!IsInCheck(pieceData.Value.IsWhite, newState))
            {
                validMoves.Add(move);
            }
        }
        return validMoves;
    }

    public bool IsInCheck(bool isWhitePlayer, BoardState boardState)
    {
        Vector2Int kingPosition = FindKingPosition(isWhitePlayer, boardState);
        if (kingPosition == new Vector2Int(-1, -1)) return false;
        return IsSquareAttacked(kingPosition, !isWhitePlayer, boardState);
    }

    public bool IsCheckmate(bool isWhitePlayer, BoardState boardState)
    {
        if (!IsInCheck(isWhitePlayer, boardState)) return false;
        return !HasAnyValidMoves(isWhitePlayer, boardState);
    }

    public bool IsStalemate(bool isWhitePlayer, BoardState boardState)
    {
        if (IsInCheck(isWhitePlayer, boardState)) return false;
        return !HasAnyValidMoves(isWhitePlayer, boardState);
    }

    public bool HasInsufficientMaterial(BoardState boardState)
    {
        var pieces = new List<BoardState.PieceData?>();
        for (int x = 0; x < Constants.BOARD_SIZE; x++)
            for (int y = 0; y < Constants.BOARD_SIZE; y++)
                if (boardState.Pieces[x, y] != null) pieces.Add(boardState.Pieces[x, y]);
        if (pieces.Count <= 2) return true;
        if (pieces.Count == 3 && (pieces.Any(p => p.Value.Type == PieceType.Knight) || pieces.Any(p => p.Value.Type == PieceType.Bishop))) return true;
        return false;
    }

    public bool IsSquareAttacked(Vector2Int square, bool byWhitePlayer, BoardState boardState)
    {
        for (int x = 0; x < Constants.BOARD_SIZE; x++)
        {
            for (int y = 0; y < Constants.BOARD_SIZE; y++)
            {
                var pieceData = boardState.Pieces[x, y];
                if (pieceData != null && pieceData.Value.IsWhite == byWhitePlayer)
                {
                    PieceLogic pieceLogic = GetPieceLogic(pieceData.Value, new Vector2Int(x, y));
                    if (pieceLogic != null && pieceLogic.GetAttackMoves(boardState).Contains(square))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    
    private bool HasAnyValidMoves(bool isWhitePlayer, BoardState boardState)
    {
        for (int x = 0; x < Constants.BOARD_SIZE; x++)
            for (int y = 0; y < Constants.BOARD_SIZE; y++)
            {
                var pieceData = boardState.Pieces[x, y];
                if (pieceData != null && pieceData.Value.IsWhite == isWhitePlayer)
                    if (GetValidMoves(new Vector2Int(x, y), boardState).Any()) return true;
            }
        return false;
    }

    private Vector2Int FindKingPosition(bool isWhitePlayer, BoardState boardState)
    {
        for (int x = 0; x < Constants.BOARD_SIZE; x++)
            for (int y = 0; y < Constants.BOARD_SIZE; y++)
            {
                var pieceData = boardState.Pieces[x, y];
                if (pieceData != null && pieceData.Value.IsWhite == isWhitePlayer && pieceData.Value.Type == PieceType.King)
                    return new Vector2Int(x, y);
            }
        return new Vector2Int(-1, -1);
    }

    private void SimulateMoveOnState(BoardState state, MoveData move)
    {
        // Basic validation to prevent errors.
        if (move.From.x < 0 || move.From.x >= 8 || move.To.x < 0 || move.To.x >= 8) return;
        
        var pieceData = state.Pieces[move.From.x, move.From.y];
        if (!pieceData.HasValue) return;

        var movedPiece = pieceData.Value;
        movedPiece.HasMoved = true;

        // --- SPECIAL MOVE HANDLING ---

        // 1. Castling
        if (movedPiece.Type == PieceType.King && Mathf.Abs(move.To.x - move.From.x) == 2)
        {
            // Move the King
            state.Pieces[move.To.x, move.To.y] = movedPiece;
            state.Pieces[move.From.x, move.From.y] = null;

            // Now, find and move the corresponding Rook.
            Vector2Int rookFromPos, rookToPos;
            if (move.To.x > move.From.x) // Kingside
            {
                rookFromPos = new Vector2Int(7, move.From.y);
                rookToPos = new Vector2Int(move.To.x - 1, move.From.y);
            }
            else // Queenside
            {
                rookFromPos = new Vector2Int(0, move.From.y);
                rookToPos = new Vector2Int(move.To.x + 1, move.From.y);
            }

            var rookData = state.Pieces[rookFromPos.x, rookFromPos.y];
            if (rookData.HasValue)
            {
                var movedRook = rookData.Value;
                movedRook.HasMoved = true; // Set the Rook's HasMoved flag!
                state.Pieces[rookToPos.x, rookToPos.y] = movedRook;
                state.Pieces[rookFromPos.x, rookFromPos.y] = null;
            }
        }
        // 2. En Passant (You may need to add this if you haven't already)
        else if (movedPiece.Type == PieceType.Pawn && move.To == state.EnPassantTargetSquare)
        {
            // Move the capturing pawn
            state.Pieces[move.To.x, move.To.y] = movedPiece;
            state.Pieces[move.From.x, move.From.y] = null;
            
            // Remove the captured pawn
            int captureDirection = movedPiece.IsWhite ? -1 : 1;
            Vector2Int capturedPawnPos = new Vector2Int(move.To.x, move.To.y + captureDirection);
            state.Pieces[capturedPawnPos.x, capturedPawnPos.y] = null;
        }
        // 3. Normal Move
        else
        {
            state.Pieces[move.To.x, move.To.y] = movedPiece;
            state.Pieces[move.From.x, move.From.y] = null;
        }

        // --- Update En Passant Target for the NEXT turn ---
        if (movedPiece.Type == PieceType.Pawn && Mathf.Abs(move.To.y - move.From.y) == 2)
        {
            state.EnPassantTargetSquare = new Vector2Int(move.To.x, move.To.y + (movedPiece.IsWhite ? -1 : 1));
        }
        else
        {
            state.EnPassantTargetSquare = new Vector2Int(-1, -1);
        }
    }
}