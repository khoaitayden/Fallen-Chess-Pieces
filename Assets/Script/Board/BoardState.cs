// In BoardState.cs
using UnityEngine;

public class BoardState
{
    public struct PieceData
    {
        public PieceType Type;
        public bool IsWhite;
        public bool HasMoved;
    }

    public PieceData?[,] Pieces { get; private set; }
    
    public Vector2Int EnPassantTargetSquare { get; set; }

    public BoardState()
    {
        Pieces = new PieceData?[Constants.BOARD_SIZE, Constants.BOARD_SIZE];
        EnPassantTargetSquare = new Vector2Int(-1, -1);
    }

    public BoardState(BoardState source)
    {
        Pieces = new PieceData?[Constants.BOARD_SIZE, Constants.BOARD_SIZE];
        for (int x = 0; x < Constants.BOARD_SIZE; x++)
        {
            for (int y = 0; y < Constants.BOARD_SIZE; y++)
            {
                this.Pieces[x, y] = source.Pieces[x, y];
            }
        }
        this.EnPassantTargetSquare = source.EnPassantTargetSquare;
    }
}