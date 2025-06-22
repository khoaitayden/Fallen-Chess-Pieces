using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public abstract class ChessPiece : MonoBehaviour
{
    [SerializeField] private PieceType _pieceType;
    [SerializeField] private bool _isWhite;

    public PieceType Type => _pieceType;
    public bool IsWhite => _isWhite;

    [HideInInspector]public Vector2Int _boardPosition;
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
     public void MoveTo(Vector2Int newPosition, Vector3 worldPosition)
    {
        _boardPosition = newPosition;
        _hasMoved = true;
        
        // Start a coroutine to handle the smooth visual movement
        StartCoroutine(MoveCoroutine(worldPosition));
    }

    private IEnumerator MoveCoroutine(Vector3 targetPosition)
    {
        Vector3 startPosition = transform.position;
        float timeElapsed = 0;
        float duration = 0.2f; // How long the move should take in seconds

        while (timeElapsed < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        transform.position = targetPosition; // Ensure it ends at the exact position
    }
}