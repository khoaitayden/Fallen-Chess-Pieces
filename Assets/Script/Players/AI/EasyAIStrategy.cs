// In EasyAIStrategy.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class EasyAIStrategy : IAIStrategy
{
    private static readonly System.Random _random = new System.Random();

    public MoveData GetBestMove(bool isWhite, Chessboard board)
    {
        BoardState boardState = board.CreateBoardState();
        List<MoveData> allPossibleMoves = GetAllPossibleMoves(isWhite, boardState);

        if (!allPossibleMoves.Any())
        {
            return default;
        }

        int randomIndex = _random.Next(0, allPossibleMoves.Count);
        return allPossibleMoves[randomIndex];
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