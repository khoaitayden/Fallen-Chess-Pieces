using UnityEngine;
using System.Collections.Generic;
public abstract class ChessPiece : MonoBehaviour
{
    [SerializeField] private PieceType _pieceType;
    [SerializeField] private bool _isWhite;

    public PieceType Type => _pieceType;
    public bool IsWhite => _isWhite;

    protected Vector2Int _boardPosition;
    protected bool _hasMoved = false;

    public abstract List<Vector2Int> GetPossibleMoves(Chessboard board);


    public virtual void Initialize(bool isWhite, Vector2Int startPosition)
    {
        _isWhite = isWhite;
        _boardPosition = startPosition;
        _hasMoved = false;
        //Can set colour of each piece here
    }

    public void SelectPiece()
    {
        Debug.Log($"{_pieceType} at {_boardPosition} selected.");
    }

    public void DeselectPiece()
    {
    }
}