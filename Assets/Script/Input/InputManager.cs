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
    private List<Vector2Int> _validMoves = new List<Vector2Int>(); // Valid moves for selected piece

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
        // Get mouse position in screen space
        Vector2 mousePosition = _chessControls.Player.PointerPosition.ReadValue<Vector2>();

        // Cast ray to detect what we clicked
        RaycastHit2D hit = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(mousePosition), Vector2.zero);

        if (hit.collider == null)
        {
            DeselectPiece();
            return;
        }

        // Check if we clicked on a piece
        if (hit.collider.TryGetComponent(out ChessPiece piece))
        {
            SelectPiece(piece);
        }
        // Else check if we clicked on a square while a piece is selected
        else if (_selectedPiece != null && hit.collider.TryGetComponent(out BoardSquare square))
        {
            AttemptMove(square.GetBoardPosition());
        }
    }

    private void SelectPiece(ChessPiece piece)
    {
        // Prevent selecting if it's not the player's turn
        if (piece.IsWhite != TurnManager.Instance.IsWhiteTurn)
        {
            Debug.Log("Not your turn!");
            return;
        }

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

        _validMoves.Clear();
        ClearHighlights();
    }

    private void AttemptMove(Vector2Int targetPosition)
    {
        if (_validMoves.Contains(targetPosition))
        {
            chessboard.MovePiece(_selectedPiece, targetPosition);

            // Switch turn after successful move
            TurnManager.Instance.SwitchTurn();

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
        foreach (Vector2Int move in _validMoves)
        {
            BoardSquare square = chessboard.GetSquareAt(move);
            if (square != null)
            {
                square.SetHighlight(true);
            }
        }
    }

    private void ClearHighlights()
    {
        for (int x = 0; x < Constants.BOARD_SIZE; x++)
        {
            for (int y = 0; y < Constants.BOARD_SIZE; y++)
            {
                BoardSquare square = chessboard.GetSquareAt(new Vector2Int(x, y));
                if (square != null)
                {
                    square.SetHighlight(false);
                }
            }
        }
    }
}