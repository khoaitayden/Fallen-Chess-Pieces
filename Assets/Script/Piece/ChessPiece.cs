using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public abstract class ChessPiece : MonoBehaviour, IClickable
{
    [SerializeField] private PieceType _pieceType;
    [SerializeField] private bool _isWhite;

    public PieceType Type => _pieceType;


    [HideInInspector] public Vector2Int _boardPosition;
    public bool _hasMoved = false;
    public bool IsWhite => _isWhite;

    public virtual void Initialize(bool isWhite, Vector2Int startPosition)
    {
        _isWhite = isWhite;
        _boardPosition = startPosition;
        _hasMoved = false;
        //Can set colour of each piece here
    }
    private void LateUpdate()
    {
        transform.rotation = Quaternion.identity;
    }

    public void SelectPiece()
    {
        Debug.Log($"{_pieceType} at {_boardPosition} selected.");
    }
    public abstract List<Vector2Int> GetPossibleMoves(Chessboard board);
    public abstract List<Vector2Int> GetAttackMoves(Chessboard board);
    public void DeselectPiece()
    {
    }

    public Vector2Int GetBoardPosition()
    {
        return _boardPosition;
    }

    

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