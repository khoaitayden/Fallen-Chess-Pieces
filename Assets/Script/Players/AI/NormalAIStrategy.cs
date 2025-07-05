// In NormalAIStrategy.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NormalAIStrategy : IAIStrategy
{
    public MoveData GetBestMove(bool isWhite, Chessboard board)
    {
        MoveData bestMove = default;
        int bestMoveScore = int.MinValue;

        // --- GATHER ALL POSSIBLE MOVES ---
        List<MoveData> allPossibleMoves = new List<MoveData>();
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
                        string notation = MoveConverter.ToDescriptiveNotation(piece, move);
                        allPossibleMoves.Add(new MoveData(piece.Type, piece._boardPosition, move, notation));
                    }
                }
            }
        }

        // If there are no moves, return an empty one.
        if (!allPossibleMoves.Any()) return default;

        // --- EVALUATE EACH MOVE ---
        foreach (var move in allPossibleMoves)
        {
            int currentMoveScore = 0;

            // 1. Check for captures
            ChessPiece pieceAtTarget = board.GetPieceAt(move.To);
            if (pieceAtTarget != null)
            {
                // Score is the value of the piece we are capturing.
                currentMoveScore = PieceValues.Values[pieceAtTarget.Type];
            }

            // 2. Simulate the move to check for checkmate (highest priority)
            ChessPiece pieceToMove = board.GetPieceAt(move.From);
            ChessPiece capturedPiece = board.SimulateMove(pieceToMove, move.To);

            // Check if this move results in checkmate against the opponent.
            bool opponentIsWhite = !isWhite;
            if (MoveValidator.Instance.IsCheckmate(opponentIsWhite))
            {
                currentMoveScore = int.MaxValue; // Checkmate is the best possible outcome.
            }

            // Undo the simulation to restore the board state for the next evaluation.
            board.UndoSimulatedMove(pieceToMove, move.From, capturedPiece);


            // 3. Compare with the best move found so far.
            if (currentMoveScore > bestMoveScore)
            {
                bestMoveScore = currentMoveScore;
                bestMove = move;
            }
        }

        // If after checking all moves, no move had a score > 0 (e.g., no captures),
        // then just make a random move.
        if (bestMoveScore == 0)
        {
            Debug.Log("Normal AI: No high-value move found. Making a random move.");
            return allPossibleMoves[Random.Range(0, allPossibleMoves.Count)];
        }

        Debug.Log($"Normal AI: Chose move with score {bestMoveScore}");
        return bestMove;
    }
}