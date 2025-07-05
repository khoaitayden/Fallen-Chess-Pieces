using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MoveValidator : MonoBehaviour
{
    public static MoveValidator Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    // --- PUBLIC METHODS FOR LIVE GAME ---

    public List<Vector2Int> GetValidMoves(ChessPiece piece)
    {
        return GetValidMoves(piece._boardPosition, Chessboard.Instance.CreateBoardState());
    }

    public bool IsInCheck(bool isWhitePlayer)
    {
        return IsInCheck(isWhitePlayer, Chessboard.Instance.CreateBoardState());
    }

    public bool IsCheckmate(bool isWhitePlayer)
    {
        return IsCheckmate(isWhitePlayer, Chessboard.Instance.CreateBoardState());
    }

    public bool IsStalemate(bool isWhitePlayer)
    {
        return IsStalemate(isWhitePlayer, Chessboard.Instance.CreateBoardState());
    }

    public bool CanCastleKingside(ChessPiece king)
    {
        return CanCastle(king, true, Chessboard.Instance.CreateBoardState());
    }

    public bool CanCastleQueenside(ChessPiece king)
    {
        return CanCastle(king, false, Chessboard.Instance.CreateBoardState());
    }

    public bool HasInsufficientMaterial()
    {
        return HasInsufficientMaterial(Chessboard.Instance.CreateBoardState());
    }

    // --- CORE LOGIC METHODS FOR SIMULATION ---

    public List<Vector2Int> GetValidMoves(Vector2Int piecePosition, BoardState boardState)
    {
        var validMoves = new List<Vector2Int>();
        var pieceData = boardState.Pieces[piecePosition.x, piecePosition.y];
        if (pieceData == null) return validMoves;

        var possibleMoves = GetPossibleMovesForPiece(piecePosition, boardState);

        foreach (var move in possibleMoves)
        {
            BoardState newState = new BoardState(boardState);
            SimulateMoveOnState(newState, new MoveData(default, piecePosition, move, ""));

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

    // --- HELPER METHODS ---

    private bool HasAnyValidMoves(bool isWhitePlayer, BoardState boardState)
    {
        for (int x = 0; x < Constants.BOARD_SIZE; x++)
        {
            for (int y = 0; y < Constants.BOARD_SIZE; y++)
            {
                var pieceData = boardState.Pieces[x, y];
                if (pieceData != null && pieceData.Value.IsWhite == isWhitePlayer)
                {
                    if (GetValidMoves(new Vector2Int(x, y), boardState).Any())
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private bool IsSquareAttacked(Vector2Int square, bool byWhitePlayer, BoardState boardState)
    {
        for (int x = 0; x < Constants.BOARD_SIZE; x++)
        {
            for (int y = 0; y < Constants.BOARD_SIZE; y++)
            {
                var pieceData = boardState.Pieces[x, y];
                if (pieceData != null && pieceData.Value.IsWhite == byWhitePlayer)
                {
                    if (GetAttackMovesForPiece(new Vector2Int(x, y), boardState).Contains(square))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private Vector2Int FindKingPosition(bool isWhitePlayer, BoardState boardState)
    {
        for (int x = 0; x < Constants.BOARD_SIZE; x++)
        {
            for (int y = 0; y < Constants.BOARD_SIZE; y++)
            {
                var pieceData = boardState.Pieces[x, y];
                if (pieceData != null && pieceData.Value.IsWhite == isWhitePlayer && pieceData.Value.Type == PieceType.King)
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return new Vector2Int(-1, -1);
    }

    private bool CanCastle(ChessPiece king, bool isKingside, BoardState boardState)
    {
        var kingPos = king._boardPosition;
        var kingData = boardState.Pieces[kingPos.x, kingPos.y];
        if (kingData == null || kingData.Value.HasMoved || IsInCheck(king.IsWhite, boardState)) return false;

        int rookFile = isKingside ? 7 : 0;
        var rookData = boardState.Pieces[rookFile, kingPos.y];
        if (rookData == null || rookData.Value.Type != PieceType.Rook || rookData.Value.HasMoved) return false;

        int direction = isKingside ? 1 : -1;
        for (int i = 1; i < (isKingside ? 3 : 4); i++)
        {
            if (boardState.Pieces[kingPos.x + i * direction, kingPos.y] != null) return false;
        }

        for (int i = 1; i < 3; i++)
        {
            if (IsSquareAttacked(new Vector2Int(kingPos.x + i * direction, kingPos.y), !king.IsWhite, boardState)) return false;
        }

        return true;
    }

    public bool HasInsufficientMaterial(BoardState boardState)
    {
        var pieces = new List<PieceType>();
        for (int x = 0; x < Constants.BOARD_SIZE; x++)
        {
            for (int y = 0; y < Constants.BOARD_SIZE; y++)
            {
                if (boardState.Pieces[x, y] != null)
                {
                    pieces.Add(boardState.Pieces[x, y].Value.Type);
                }
            }
        }

        if (pieces.Count <= 3) 
        {
            if (pieces.All(p => p == PieceType.King || p == PieceType.Knight || p == PieceType.Bishop))
            {
                return true;
            }
        }

        return false;
    }

    private void SimulateMoveOnState(BoardState state, MoveData move)
    {
        var pieceData = state.Pieces[move.From.x, move.From.y];
        if (pieceData.HasValue)
        {
            var movedPiece = pieceData.Value;
            movedPiece.HasMoved = true;
            state.Pieces[move.To.x, move.To.y] = movedPiece;
            state.Pieces[move.From.x, move.From.y] = null;
        }
    }

    // --- STATIC MOVE LOGIC (Better than dummy pieces) ---

    private List<Vector2Int> GetPossibleMovesForPiece(Vector2Int pos, BoardState state)
    {
        var pieceData = state.Pieces[pos.x, pos.y];
        if (pieceData == null) return new List<Vector2Int>();

        return pieceData.Value.Type switch
        {
            PieceType.Pawn => GetPawnMoves(pos, state),
            PieceType.Rook => GetRookMoves(pos, state),
            PieceType.Knight => GetKnightMoves(pos, state),
            PieceType.Bishop => GetBishopMoves(pos, state),
            PieceType.Queen => GetQueenMoves(pos, state),
            PieceType.King => GetKingMoves(pos, state),
            _ => new List<Vector2Int>()
        };
    }

    private List<Vector2Int> GetAttackMovesForPiece(Vector2Int pos, BoardState state)
    {
        var pieceData = state.Pieces[pos.x, pos.y];
        if (pieceData == null) return new List<Vector2Int>();

        if (pieceData.Value.Type == PieceType.Pawn)
        {
            return GetPawnAttackMoves(pos, state);
        }
        return GetPossibleMovesForPiece(pos, state);
    }

    // --- PIECE-SPECIFIC MOVE LOGIC ---

    private List<Vector2Int> GetPawnMoves(Vector2Int pos, BoardState state)
    {
        var moves = new List<Vector2Int>();
        var pieceData = state.Pieces[pos.x, pos.y].Value;
        int direction = pieceData.IsWhite ? 1 : -1;
        int startingRank = pieceData.IsWhite ? 1 : 6;

        // Forward move
        Vector2Int oneStep = new Vector2Int(pos.x, pos.y + direction);
        if (IsOnBoard(oneStep) && state.Pieces[oneStep.x, oneStep.y] == null)
        {
            moves.Add(oneStep);

            // Two-step move from starting position
            if (pos.y == startingRank)
            {
                Vector2Int twoStep = new Vector2Int(pos.x, pos.y + 2 * direction);
                if (IsOnBoard(twoStep) && state.Pieces[twoStep.x, twoStep.y] == null)
                {
                    moves.Add(twoStep);
                }
            }
        }

        // Capture moves
        Vector2Int[] captureSquares = { 
            new Vector2Int(pos.x - 1, pos.y + direction), 
            new Vector2Int(pos.x + 1, pos.y + direction) 
        };

        foreach (var captureSquare in captureSquares)
        {
            if (IsOnBoard(captureSquare))
            {
                var target = state.Pieces[captureSquare.x, captureSquare.y];
                if (target != null && target.Value.IsWhite != pieceData.IsWhite)
                {
                    moves.Add(captureSquare);
                }
                // TODO: Add en passant logic here if needed
            }
        }

        return moves;
    }

    private List<Vector2Int> GetPawnAttackMoves(Vector2Int pos, BoardState state)
    {
        var moves = new List<Vector2Int>();
        var pieceData = state.Pieces[pos.x, pos.y].Value;
        int direction = pieceData.IsWhite ? 1 : -1;

        Vector2Int[] attackSquares = { 
            new Vector2Int(pos.x - 1, pos.y + direction), 
            new Vector2Int(pos.x + 1, pos.y + direction) 
        };

        foreach (var square in attackSquares)
        {
            if (IsOnBoard(square))
            {
                moves.Add(square);
            }
        }

        return moves;
    }

    private List<Vector2Int> GetRookMoves(Vector2Int pos, BoardState state)
    {
        Vector2Int[] directions = { new(0, 1), new(0, -1), new(1, 0), new(-1, 0) };
        return GetSlidingMoves(pos, state, directions);
    }

    private List<Vector2Int> GetBishopMoves(Vector2Int pos, BoardState state)
    {
        Vector2Int[] directions = { new(1, 1), new(1, -1), new(-1, 1), new(-1, -1) };
        return GetSlidingMoves(pos, state, directions);
    }

    private List<Vector2Int> GetQueenMoves(Vector2Int pos, BoardState state)
    {
        Vector2Int[] directions = { 
            new(0, 1), new(0, -1), new(1, 0), new(-1, 0),
            new(1, 1), new(1, -1), new(-1, 1), new(-1, -1) 
        };
        return GetSlidingMoves(pos, state, directions);
    }

    private List<Vector2Int> GetKnightMoves(Vector2Int pos, BoardState state)
    {
        var moves = new List<Vector2Int>();
        var pieceData = state.Pieces[pos.x, pos.y].Value;

        Vector2Int[] knightMoves = {
            new(2, 1), new(2, -1), new(-2, 1), new(-2, -1),
            new(1, 2), new(1, -2), new(-1, 2), new(-1, -2)
        };

        foreach (var move in knightMoves)
        {
            Vector2Int newPos = pos + move;
            if (IsOnBoard(newPos))
            {
                var target = state.Pieces[newPos.x, newPos.y];
                if (target == null || target.Value.IsWhite != pieceData.IsWhite)
                {
                    moves.Add(newPos);
                }
            }
        }

        return moves;
    }

    private List<Vector2Int> GetKingMoves(Vector2Int pos, BoardState state)
    {
        var moves = new List<Vector2Int>();
        var pieceData = state.Pieces[pos.x, pos.y].Value;

        Vector2Int[] kingMoves = {
            new(0, 1), new(0, -1), new(1, 0), new(-1, 0),
            new(1, 1), new(1, -1), new(-1, 1), new(-1, -1)
        };

        foreach (var move in kingMoves)
        {
            Vector2Int newPos = pos + move;
            if (IsOnBoard(newPos))
            {
                var target = state.Pieces[newPos.x, newPos.y];
                if (target == null || target.Value.IsWhite != pieceData.IsWhite)
                {
                    moves.Add(newPos);
                }
            }
        }

        // TODO: Add castling moves here if needed
        return moves;
    }

    private List<Vector2Int> GetSlidingMoves(Vector2Int pos, BoardState state, Vector2Int[] directions)
    {
        var moves = new List<Vector2Int>();
        var pieceData = state.Pieces[pos.x, pos.y].Value;

        foreach (var direction in directions)
        {
            Vector2Int current = pos + direction;
            while (IsOnBoard(current))
            {
                var target = state.Pieces[current.x, current.y];
                if (target == null)
                {
                    moves.Add(current);
                    current += direction;
                }
                else
                {
                    if (target.Value.IsWhite != pieceData.IsWhite)
                    {
                        moves.Add(current);
                    }
                    break;
                }
            }
        }

        return moves;
    }

    private bool IsOnBoard(Vector2Int pos) => pos.x >= 0 && pos.x < Constants.BOARD_SIZE && pos.y >= 0 && pos.y < Constants.BOARD_SIZE;
}