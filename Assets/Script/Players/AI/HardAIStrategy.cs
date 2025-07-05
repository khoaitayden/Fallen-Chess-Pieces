// In HardAIStrategy.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HardAIStrategy : IAIStrategy
{
    private const int SEARCH_DEPTH = 4;
    private bool _aiIsWhite;

    public MoveData GetBestMove(bool isWhite, Chessboard board)
    {
        Debug.Log("Hard AI is thinking...");
        this._aiIsWhite = isWhite;
        BoardState initialBoardState = board.CreateBoardState();
        return MinimaxRoot(initialBoardState);
    }

    private MoveData MinimaxRoot(BoardState boardState)
    {
        MoveData bestMove = default;
        int bestMoveScore = int.MinValue;

        List<MoveData> allPossibleMoves = GetAllPossibleMoves(_aiIsWhite, boardState);

        if (!allPossibleMoves.Any()) return default;

        foreach (var move in allPossibleMoves)
        {
            BoardState newState = new BoardState(boardState);
            SimulateMoveOnState(newState, move);

            int moveScore = Minimax(SEARCH_DEPTH - 1, newState, int.MinValue, int.MaxValue, false, !_aiIsWhite);

            if (moveScore > bestMoveScore)
            {
                bestMoveScore = moveScore;
                bestMove = move;
            }
        }
        Debug.Log($"Hard AI chose move with projected score: {bestMoveScore}");
        return bestMove;
    }

    private int Minimax(int depth, BoardState boardState, int alpha, int beta, bool isMaximizingPlayer, bool playerColorIsWhite)
    {
        List<MoveData> allPossibleMoves = GetAllPossibleMoves(playerColorIsWhite, boardState);

        if (depth == 0 || !allPossibleMoves.Any())
        {
            return EvaluateBoard(boardState);
        }

        if (isMaximizingPlayer)
        {
            int maxEval = int.MinValue;
            foreach (var move in allPossibleMoves)
            {
                BoardState newState = new BoardState(boardState);
                SimulateMoveOnState(newState, move);
                int eval = Minimax(depth - 1, newState, alpha, beta, false, !playerColorIsWhite);
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
                BoardState newState = new BoardState(boardState);
                SimulateMoveOnState(newState, move);
                int eval = Minimax(depth - 1, newState, alpha, beta, true, !playerColorIsWhite);
                minEval = Mathf.Min(minEval, eval);
                beta = Mathf.Min(beta, eval);
                if (beta <= alpha) break;
            }
            return minEval;
        }
    }

    private int EvaluateBoard(BoardState boardState)
    {
        int totalScore = 0;
        for (int x = 0; x < Constants.BOARD_SIZE; x++)
        {
            for (int y = 0; y < Constants.BOARD_SIZE; y++)
            {
                var pieceData = boardState.Pieces[x, y];
                if (pieceData != null)
                {
                    int pieceValue = PieceValues.Values[pieceData.Value.Type];
                    totalScore += pieceData.Value.IsWhite ? pieceValue : -pieceValue;
                }
            }
        }
        return _aiIsWhite ? totalScore : -totalScore;
    }

    // --- HELPER METHODS THAT OPERATE ON BoardState ---

    private List<MoveData> GetAllPossibleMoves(bool isWhite, BoardState boardState)
    {
        List<MoveData> allMoves = new List<MoveData>();

        // Loop through every square on the board.
        for (int x = 0; x < Constants.BOARD_SIZE; x++)
        {
            for (int y = 0; y < Constants.BOARD_SIZE; y++)
            {
                var pieceData = boardState.Pieces[x, y];

                // Check if there is a piece on the square and if it's the correct color.
                if (pieceData != null && pieceData.Value.IsWhite == isWhite)
                {
                    Vector2Int piecePosition = new Vector2Int(x, y);

                    // Ask the MoveValidator for all valid moves for this piece in the current board state.
                    List<Vector2Int> validMoves = MoveValidator.Instance.GetValidMoves(piecePosition, boardState);

                    // For each valid move, create a MoveData object and add it to our list.
                    foreach (var move in validMoves)
                    {
                        // The notation can be empty here because the AI doesn't need it for its calculations.
                        // We will generate the proper notation only for the final, chosen move.
                        allMoves.Add(new MoveData(pieceData.Value.Type, piecePosition, move, ""));
                    }
                }
            }
        }
        return allMoves;
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
}