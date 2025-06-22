using UnityEngine;
using UnityEngine.InputSystem; // Important!
using System.Collections.Generic;
public class InputManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Chessboard chessboard;

    private ChessControls _chessControls;
    private ChessPiece _selectedPiece;
    private List<Vector2Int> _validMoves; // To store valid moves for the selected piece

    private void Awake()
    {
        _chessControls = new ChessControls();
    }

    private void OnEnable()
    {
        _chessControls.Enable();
        // Subscribe our HandleClick method to the "performed" event of the Click action
        _chessControls.Player.Click.performed += HandleClick;
    }

    private void OnDisable()
    {
        _chessControls.Player.Click.performed -= HandleClick;
        _chessControls.Disable();
    }

    private void HandleClick(InputAction.CallbackContext context)
    {
        // Read the mouse position from the PointerPosition action
        Vector2 mousePosition = _chessControls.Player.PointerPosition.ReadValue<Vector2>();

        RaycastHit2D hit = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(mousePosition), Vector2.zero);

        if (hit.collider == null)
        {
            // Clicked on empty space, deselect anything
            DeselectPiece();
            return;
        }

        // Check if we clicked on a piece
        if (hit.collider.TryGetComponent(out ChessPiece piece))
        {
            SelectPiece(piece);
        }
        // Check if we clicked on a square (and a piece is already selected)
        else if (_selectedPiece != null && hit.collider.TryGetComponent(out BoardSquare square))
        {
            AttemptMove(square.GetBoardPosition());
        }
    }

    private void SelectPiece(ChessPiece piece)
    {
        DeselectPiece();

        _selectedPiece = piece;
        _selectedPiece.SelectPiece();

        _validMoves = _selectedPiece.GetPossibleMoves(chessboard);

        HighlightValidMoves();
    }

    private void DeselectPiece()
    {
        if (_selectedPiece != null)
        {
            _selectedPiece.DeselectPiece();
            _selectedPiece = null;
        }
        _validMoves?.Clear();
        ClearHighlights();
    }

    private void AttemptMove(Vector2Int targetPosition)
    {
        if (_validMoves.Contains(targetPosition))
        {
            chessboard.MovePiece(_selectedPiece, targetPosition);
            
            DeselectPiece();
        }
        else
        {
            Debug.Log($"Invalid move to {targetPosition}");
            DeselectPiece();
        }
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