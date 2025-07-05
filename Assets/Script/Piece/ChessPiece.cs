using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public abstract class ChessPiece : MonoBehaviour, IClickable
{
    [SerializeField] private PieceType _pieceType;
    [SerializeField] protected bool _isWhite;

    public PieceType Type => _pieceType;
    public bool IsWhite => _isWhite;

    [HideInInspector] public Vector2Int _boardPosition;
    public bool _hasMoved = false;

    protected PieceLogic _logic;

    public virtual void Initialize(bool isWhite, Vector2Int startPosition)
    {
        this._isWhite = isWhite;
        _boardPosition = startPosition;
        _hasMoved = false;
        
        CreateLogic();
    }

    protected abstract void CreateLogic();



    public List<Vector2Int> GetPossibleMoves(BoardState boardState)
    {
        _logic.Initialize(this.IsWhite, this._boardPosition, this._hasMoved, this.Type);
        return _logic.GetPossibleMoves(boardState);
    }

    public List<Vector2Int> GetAttackMoves(BoardState boardState)
    {
        _logic.Initialize(this.IsWhite, this._boardPosition, this._hasMoved, this.Type);
        return _logic.GetAttackMoves(boardState);
    }



    private void LateUpdate() 
    {
        transform.rotation = Quaternion.identity; 
    }

    public void SelectPiece() 
    { 
        Debug.Log($"{_pieceType} at {_boardPosition} selected."); 
    }

    public void DeselectPiece() { }

    public Vector2Int GetBoardPosition() => _boardPosition;

    public void MoveTo(Vector2Int newPosition, Vector3 targetLocalPosition)
    {
        _boardPosition = newPosition;
        _hasMoved = true;
        StartCoroutine(MoveCoroutine(targetLocalPosition));
    }

    private IEnumerator MoveCoroutine(Vector3 targetLocalPosition)
    {
        Vector3 startPosition = transform.localPosition;
        float timeElapsed = 0;
        float duration = 0.3f;
        while (timeElapsed < duration)
        {
            transform.localPosition = Vector3.Lerp(startPosition, targetLocalPosition, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = targetLocalPosition;
    }
}