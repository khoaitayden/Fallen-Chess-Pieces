using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ChessPiece : MonoBehaviour, IClickable
{
    [Header("Piece Configuration")]
    [Tooltip("Set the type of this piece in the Inspector.")]
    [SerializeField] private PieceType _pieceType;
    [SerializeField] private bool _isWhite;

    public PieceType Type => _pieceType;
    public bool IsWhite => _isWhite;

    [HideInInspector] public Vector2Int _boardPosition;
    public bool _hasMoved = false;

    // The "brain" for this piece.
    private PieceLogic _logic;

    public void Initialize(bool isWhite, Vector2Int startPosition)
    {
        this._isWhite = isWhite;
        _boardPosition = startPosition;
        _hasMoved = false;
        
        // Create the correct brain based on our type.
        CreateLogic();
    }

    // This "Factory" method creates the correct logic instance.
    private void CreateLogic()
    {
        switch (_pieceType)
        {
            // --- Standard Pieces ---
            case PieceType.Pawn:   _logic = new PawnLogic();   break;
            case PieceType.Rook:   _logic = new RookLogic();   break;
            case PieceType.Knight: _logic = new KnightLogic(); break;
            case PieceType.Bishop: _logic = new BishopLogic(); break;
            case PieceType.Queen:  _logic = new QueenLogic();  break;
            case PieceType.King:   _logic = new KingLogic();   break;

            // --- ADD THESE NEW CASES for Combination Pieces ---
            case PieceType.KnightRook:         _logic = new KnightRookLogic();         break;
            case PieceType.KnightBishop:       _logic = new KnightBishopLogic();       break;
            case PieceType.RookBishop:         _logic = new RookBishopLogic();         break; // This is a QueenLogic
            case PieceType.KnightBishopRook:   _logic = new KnightBishopRookLogic();   break;
            // ----------------------------------------------------

            default:
                Debug.LogError($"No logic found in ChessPiece.CreateLogic for type {_pieceType}");
                break;
        }
    }

    // These methods act as a bridge to the piece's "brain".
    // They ensure the logic is always updated with the piece's current state.
    public List<Vector2Int> GetPossibleMoves(BoardState boardState)
    {
        // Safety check
        if (_logic == null) {
            Debug.LogError($"Piece {this.name} at {_boardPosition} has a null logic brain!");
            return new List<Vector2Int>();
        }
        _logic.Initialize(this.IsWhite, this._boardPosition, this._hasMoved, this.Type);
        return _logic.GetPossibleMoves(boardState);
    }

    public List<Vector2Int> GetAttackMoves(BoardState boardState)
    {
        // Safety check
        if (_logic == null) {
            Debug.LogError($"Piece {this.name} at {_boardPosition} has a null logic brain!");
            return new List<Vector2Int>();
        }
        _logic.Initialize(this.IsWhite, this._boardPosition, this._hasMoved, this.Type);
        return _logic.GetAttackMoves(boardState);
    }

    // --- The rest of the MonoBehaviour logic is unchanged ---

    private void LateUpdate() { transform.rotation = Quaternion.identity; }
    public void SelectPiece() { Debug.Log($"{_pieceType} at {_boardPosition} selected."); }
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