using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Chessboard chessboard;

    private ChessControls _chessControls;
    private ChessPiece _selectedPiece;
    private List<Vector2Int> _validMoves;

    private void Awake()
    {
        _chessControls = new ChessControls();
    }

    private void OnEnable()
    {
        _chessControls.Enable();
        _chessControls.Player.Click.performed += HandleClick;
    }

    private void OnDisable()
    {
        _chessControls.Player.Click.performed -= HandleClick;
        _chessControls.Disable();
    }

    private void HandleClick(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.CurrentState != GameState.Playing) return;

        Vector2Int clickedPosition = GetClickedBoardPosition();
        if (clickedPosition == new Vector2Int(-1, -1))
        {
            if (_selectedPiece != null) DeselectPiece();
            return;
        }

        if (_selectedPiece == null)
        {
            AttemptSelection(clickedPosition);
        }
        else
        {
            HandleAction(clickedPosition);
        }
    }
    
    private void AttemptSelection(Vector2Int position)
    {
        ChessPiece piece = chessboard.GetPieceAt(position);
        if (piece != null && piece.IsWhite == TurnManager.Instance.IsWhiteTurn)
        {
            SelectPiece(piece);
        }
    }

    private void HandleAction(Vector2Int position)
    {
        // Action 1: Try to move to the clicked square.
        if (_validMoves.Contains(position))
        {
            chessboard.MovePiece(_selectedPiece, position);
            TurnManager.Instance.SwitchTurn();
            DeselectPiece();
            return;
        }

        // Action 2: Clicked on a different friendly piece. Switch selection.
        ChessPiece clickedPiece = chessboard.GetPieceAt(position);
        if (clickedPiece != null && clickedPiece.IsWhite == _selectedPiece.IsWhite)
        {
            DeselectPiece();
            SelectPiece(clickedPiece);
            return;
        }

        // Action 3: Clicked anywhere else (invalid square, enemy piece not in a valid move). Deselect.
        DeselectPiece();
    }

    private void SelectPiece(ChessPiece piece)
    {
        _selectedPiece = piece;
        _validMoves = MoveValidator.Instance.GetValidMoves(_selectedPiece);
        HighlightValidMoves();
        _selectedPiece.SelectPiece();
    }

    private void DeselectPiece()
    {
        if (_selectedPiece == null) return;

        _selectedPiece.DeselectPiece();
        _selectedPiece = null;
        _validMoves?.Clear();
        ClearHighlights();
    }

    private Vector2Int GetClickedBoardPosition()
    {
        Vector2 mousePosition = _chessControls.Player.PointerPosition.ReadValue<Vector2>();
        RaycastHit2D hit = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(mousePosition), Vector2.zero);

        if (hit.collider == null) return new Vector2Int(-1, -1);

        BoardSquare square = hit.collider.GetComponent<BoardSquare>();
        if (square != null) return square.GetBoardPosition();

        ChessPiece piece = hit.collider.GetComponent<ChessPiece>();
        if (piece != null) return piece._boardPosition;

        return new Vector2Int(-1, -1);
    }

    private void HighlightValidMoves()
    {
        if (_validMoves == null) return;
        foreach (Vector2Int move in _validMoves)
        {
            chessboard.GetSquareAt(move)?.SetHighlight(true);
        }
    }

    private void ClearHighlights()
    {
        for (int x = 0; x < Constants.BOARD_SIZE; x++)
        {
            for (int y = 0; y < Constants.BOARD_SIZE; y++)
            {
                chessboard.GetSquareAt(new Vector2Int(x, y))?.SetHighlight(false);
            }
        }
    }
}