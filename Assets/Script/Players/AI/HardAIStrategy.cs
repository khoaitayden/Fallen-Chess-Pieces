// In HardAIStrategy.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HardAIStrategy : IAIStrategy
{
    private const int SEARCH_DEPTH = 4;

    public MoveData GetBestMove(bool isWhite, Chessboard board)
    {
        Debug.Log("Hard AI is thinking...");
        return MinimaxRoot(isWhite, board);
    }

    private MoveData MinimaxRoot(bool isWhite, Chessboard board)
    {
        MoveData bestMove = default;
        int bestMoveScore = int.MinValue;

        // Use the new helper method to get all moves.
        List<MoveData> allPossibleMoves = GetAllPossibleMoves(isWhite, board);

        // If there are no moves, the game is over (stalemate/checkmate).
        if (!allPossibleMoves.Any()) return default;

        foreach (var move in allPossibleMoves)
        {
            ChessPiece pieceToMove = board.GetPieceAt(move.From);
            ChessPiece capturedPiece = board.SimulateMove(pieceToMove, move.To);
            
            int moveScore = Minimax(SEARCH_DEPTH - 1, board, int.MinValue, int.MaxValue, false, !isWhite);
            
            board.UndoSimulatedMove(pieceToMove, move.From, capturedPiece);

            if (moveScore > bestMoveScore)
            {
                bestMoveScore = moveScore;
                bestMove = move;
            }
        }
        Debug.Log($"Hard AI chose move with projected score: {bestMoveScore}");
        return bestMove;
    }

    private int Minimax(int depth, Chessboard board, int alpha, int beta, bool isMaximizingPlayer, bool playerColorIsWhite)
    {
        // Get all moves for the current player in the simulation.
        List<MoveData> allPossibleMoves = GetAllPossibleMoves(playerColorIsWhite, board);

        // Base Case: If depth is 0 OR there are no legal moves (checkmate/stalemate), evaluate the board.
        if (depth == 0 || !allPossibleMoves.Any())
        {
            return EvaluateBoard(board, playerColorIsWhite);
        }

        if (isMaximizingPlayer)
        {
            int maxEval = int.MinValue;
            foreach (var move in allPossibleMoves)
            {
                ChessPiece pieceToMove = board.GetPieceAt(move.From);
                ChessPiece capturedPiece = board.SimulateMove(pieceToMove, move.To);
                int eval = Minimax(depth - 1, board, alpha, beta, false, !playerColorIsWhite);
                board.UndoSimulatedMove(pieceToMove, move.From, capturedPiece);
                maxEval = Mathf.Max(maxEval, eval);
                alpha = Mathf.Max(alpha, eval);
                if (beta <= alpha) break;
            }
            return maxEval;
        }
        else // Minimizing Player
        {
            int minEval = int.MaxValue;
            foreach (var move in allPossibleMoves)
            {
                ChessPiece pieceToMove = board.GetPieceAt(move.From);
                ChessPiece capturedPiece = board.SimulateMove(pieceToMove, move.To);
                int eval = Minimax(depth - 1, board, alpha, beta, true, !playerColorIsWhite);
                board.UndoSimulatedMove(pieceToMove, move.From, capturedPiece);
                minEval = Mathf.Min(minEval, eval);
                beta = Mathf.Min(beta, eval);
                if (beta <= alpha) break;
            }
            return minEval;
        }
    }

    // The static evaluation function.
    private int EvaluateBoard(Chessboard board, bool playerColorIsWhite)
    {
        int totalScore = 0;
        for (int x = 0; x < Constants.BOARD_SIZE; x++)
        {
            for (int y = 0; y < Constants.BOARD_SIZE; y++)
            {
                ChessPiece piece = board.GetPieceAt(new Vector2Int(x, y));
                if (piece != null)
                {
                    int pieceValue = PieceValues.Values[piece.Type];
                    totalScore += piece.IsWhite ? pieceValue : -pieceValue;
                }
            }
        }
        // The score is from White's perspective. If the current player is Black, we negate it.
        return playerColorIsWhite ? totalScore : -totalScore;
    }

    // --- THIS IS THE MISSING PIECE OF LOGIC ---
    private List<MoveData> GetAllPossibleMoves(bool isWhite, Chessboard board)
    {
        List<MoveData> allMoves = new List<MoveData>();
        for (int x = 0; x < Constants.BOARD_SIZE; x++)
        {
            for (int y = 0; y < Constants.BOARD_SIZE; y++)
            {
                ChessPiece piece = board.GetPieceAt(new Vector2Int(x, y));
                if (piece != null && piece.IsWhite == isWhite)
                {
                    List<Vector2Int> validMoves = MoveValidator.Instance.GetValidMoves(piece);
                    foreach (var move in validMoves)
                    {
                        // We don't need perfect notation here, just the move data.
                        allMoves.Add(new MoveData(piece.Type, piece._boardPosition, move, ""));
                    }
                }
            }
        }
        return allMoves;
    }
}