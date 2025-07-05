using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class NormalAIStrategy : IAIStrategy
{
    private static readonly System.Random _random = new System.Random();

    public MoveData GetBestMove(bool isWhite, Chessboard board)
    {
        BoardState boardState = board.CreateBoardState();
        List<MoveData> allPossibleMoves = GetAllPossibleMoves(isWhite, boardState);

        if (!allPossibleMoves.Any()) return default;

        MoveData bestMove = default;
        int bestMoveScore = -1;

        foreach (var move in allPossibleMoves)
        {
            int currentMoveScore = 0;
            var pieceAtTarget = boardState.Pieces[move.To.x, move.To.y];
            if (pieceAtTarget != null)
            {
                currentMoveScore = PieceValues.Values[pieceAtTarget.Value.Type];
            }

            BoardState newState = new BoardState(boardState);
            SimulateMoveOnState(newState, move);

            if (MoveValidator.Instance.IsCheckmate(!isWhite, newState))
            {
                currentMoveScore = int.MaxValue;
            }

            if (currentMoveScore > bestMoveScore)
            {
                bestMoveScore = currentMoveScore;
                bestMove = move;
            }
        }

        if (bestMoveScore <= 0)
        {
            int randomIndex = _random.Next(0, allPossibleMoves.Count);
            return allPossibleMoves[randomIndex];
        }

        return bestMove;
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

    private List<MoveData> GetAllPossibleMoves(bool isWhite, BoardState boardState)
    {
        List<MoveData> allMoves = new List<MoveData>();
        for (int x = 0; x < Constants.BOARD_SIZE; x++)
        {
            for (int y = 0; y < Constants.BOARD_SIZE; y++)
            {
                var pieceData = boardState.Pieces[x, y];
                if (pieceData != null && pieceData.Value.IsWhite == isWhite)
                {
                    Vector2Int piecePosition = new Vector2Int(x, y);
                    List<Vector2Int> validMoves = MoveValidator.Instance.GetValidMoves(piecePosition, boardState);
                    foreach (var move in validMoves)
                    {
                        allMoves.Add(new MoveData(pieceData.Value.Type, piecePosition, move, ""));
                    }
                }
            }
        }
        return allMoves;
    }
}