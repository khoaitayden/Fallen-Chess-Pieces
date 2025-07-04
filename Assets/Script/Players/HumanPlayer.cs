// In HumanPlayer.cs
using System.Collections.Generic;
using UnityEngine;

public class HumanPlayer : Player
{
    private ChessPiece _selectedPiece;
    private List<Vector2Int> _validMoves;
    private Chessboard _chessboard;

    public HumanPlayer(bool isWhite) : base(isWhite, PlayerType.Human)
    {
        _chessboard = Object.FindObjectOfType<Chessboard>();
    }

    public override void OnTurnStart()
    {
        InputController.Instance.OnBoardClick += HandleBoardClick;
    }

    private void HandleBoardClick(Vector2Int position)
    {
        if (GameManager.Instance.CurrentState != GameState.Playing) return;

        // Scenario 1: A piece is already selected
        if (_selectedPiece != null)
        {
            // If the clicked position is a valid move, make the move.
            if (_validMoves != null && _validMoves.Contains(position))
            {
                MakeMove(position);
                return;
            }

            // If not a valid move, check if we are selecting another friendly piece.
            ChessPiece pieceAtPos = _chessboard.GetPieceAt(position);
            if (pieceAtPos != null && pieceAtPos.IsWhite == this.IsWhite)
            {
                DeselectPiece();
                SelectPiece(pieceAtPos);
                return;
            }

            // Otherwise, just deselect.
            DeselectPiece();
        }
        // Scenario 2: No piece is selected, try to select one.
        else
        {
            AttemptSelection(position);
        }
    }

    /// <summary>
    /// This is the single, reliable place where a move is executed and the turn ends.
    /// </summary>
    private void MakeMove(Vector2Int toPosition)
    {
        // 1. Unsubscribe from input immediately. This player's job is done.
        InputController.Instance.OnBoardClick -= HandleBoardClick;

        // 2. Gather info and execute the move on the board.
        Vector2Int oldPosition = _selectedPiece._boardPosition;
        _chessboard.MovePiece(_selectedPiece, toPosition);
        TurnManager.Instance.SetEnPassantTarget(_selectedPiece, oldPosition, toPosition);

        // 3. Switch the logical turn.
        TurnManager.Instance.SwitchTurn();

        // 4. Record the move in history.
        string notation = MoveConverter.ToDescriptiveNotation(_selectedPiece, toPosition);
        MoveData move = new MoveData(_selectedPiece.Type, oldPosition, toPosition, notation);
        MoveHistory.Instance.AddMove(move);

        // 5. Clean up selection and check for game end.
        DeselectPiece();
        GameManager.Instance.CheckForGameEnd();

        // 6. If the game is still going, notify the next player to start their turn.
        if (GameManager.Instance.CurrentState == GameState.Playing)
        {
            GameManager.Instance.NotifyCurrentPlayer();
        }
    }

    private void AttemptSelection(Vector2Int position)
    {
        if (position == new Vector2Int(-1, -1)) return;
        ChessPiece piece = _chessboard.GetPieceAt(position);
        if (piece != null && piece.IsWhite == this.IsWhite)
        {
            SelectPiece(piece);
        }
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

    private void HighlightValidMoves()
    {
        if (_validMoves == null) return;
        foreach (Vector2Int move in _validMoves)
        {
            _chessboard.GetSquareAt(move)?.SetHighlight(true);
        }
    }

    private void ClearHighlights()
    {
        for (int x = 0; x < Constants.BOARD_SIZE; x++)
        {
            for (int y = 0; y < Constants.BOARD_SIZE; y++)
            {
                _chessboard.GetSquareAt(new Vector2Int(x, y))?.SetHighlight(false);
            }
        }
    }
}
