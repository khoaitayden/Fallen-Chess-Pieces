// Create new script: EasyAIStrategy.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EasyAIStrategy : IAIStrategy
{
    public MoveData GetBestMove(bool isWhite, Chessboard board)
    {
        List<ChessPiece> movablePieces = new List<ChessPiece>();
        for (int x = 0; x < Constants.BOARD_SIZE; x++)
        {
            for (int y = 0; y < Constants.BOARD_SIZE; y++)
            {
                ChessPiece piece = board.GetPieceAt(new Vector2Int(x, y));
                if (piece != null && piece.IsWhite == isWhite)
                {
                    if (MoveValidator.Instance.GetValidMoves(piece).Any())
                    {
                        movablePieces.Add(piece);
                    }
                }
            }
        }

        if (!movablePieces.Any())
        {

            return default;
        }

        ChessPiece randomPiece = movablePieces[Random.Range(0, movablePieces.Count)];
        
        List<Vector2Int> validMoves = MoveValidator.Instance.GetValidMoves(randomPiece);
        Vector2Int randomMoveTo = validMoves[Random.Range(0, validMoves.Count)];

        string notation = MoveConverter.ToDescriptiveNotation(randomPiece, randomMoveTo);
        return new MoveData(randomPiece.Type, randomPiece._boardPosition, randomMoveTo, notation);
    }
}