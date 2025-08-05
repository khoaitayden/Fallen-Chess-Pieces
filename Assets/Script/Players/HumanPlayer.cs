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

    public override void OnTurnEnd()
    {
        InputController.Instance.OnBoardClick -= HandleBoardClick;
        DeselectPiece();
    }

    private void HandleBoardClick(Vector2Int position)
    {
        GameState currentState = GameManager.Instance.CurrentState;
        if (currentState == GameState.Playing)
        {
            HandlePlayStateClick(position);
        }
        else if (currentState == GameState.PowerTransfer)
        {
            HandlePowerTransferClick(position);
        }
    }

    private void HandlePlayStateClick(Vector2Int position)
    {
        if (_selectedPiece != null)
        {
            if (_validMoves != null && _validMoves.Contains(position))
            {
                GameManager.Instance.ProcessMove(_selectedPiece, _selectedPiece._boardPosition, position);
                return;
            }
            ChessPiece pieceAtPos = _chessboard.GetPieceAt(position);
            if (pieceAtPos != null && pieceAtPos.IsWhite == this.IsWhite)
            {
                DeselectPiece();
                SelectPiece(pieceAtPos);
            }
            else
            {
                DeselectPiece();
            }
        }
        else
        {
            AttemptSelection(position);
        }
    }

    private void HandlePowerTransferClick(Vector2Int position)
    {
        if (position == new Vector2Int(-1, -1)) return;
        ChessPiece clickedPiece = _chessboard.GetPieceAt(position);

        if (clickedPiece != null &&
            clickedPiece.IsWhite == this.IsWhite &&
            clickedPiece.Type != PieceType.King &&
            clickedPiece.Type != PieceType.Queen &&
            clickedPiece.Type != PieceType.Pawn)
        {
            ClearHighlights();
            GameManager.Instance.CompletePowerTransfer(clickedPiece);
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
        _validMoves = MoveValidator.Instance.GetValidMoves(piece);
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

    public void HighlightPowerTargets()
    {
        ClearHighlights();
        List<ChessPiece> validTargets = GameManager.Instance.FindValidPowerTargets(this.IsWhite);
        foreach (ChessPiece piece in validTargets)
        {
            _chessboard.GetSquareAt(piece._boardPosition)?.SetHighlight(true, HighlightColors.PowerTarget);
        }
    }

    private void HighlightValidMoves()
    {
        if (_validMoves == null) return;
        foreach (Vector2Int move in _validMoves)
        {
            _chessboard.GetSquareAt(move)?.SetHighlight(true, HighlightColors.ValidMove);
        }
    }

    private void ClearHighlights()
    {
        for (int x = 0; x < Constants.BOARD_SIZE; x++)
        {
            for (int y = 0; y < Constants.BOARD_SIZE; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                BoardSquare square = _chessboard.GetSquareAt(pos);
                square?.SetHighlight(false, Color.clear);
            }
        }
    }

    public void EnablePowerTransferInput()
    {
        InputController.Instance.OnBoardClick -= HandleBoardClick;
        InputController.Instance.OnBoardClick += HandleBoardClick;
    }

    public void DisablePowerTransferInput()
    {
        InputController.Instance.OnBoardClick -= HandleBoardClick;
    }
    
    public void ClearAllHighlights()
    {
        ClearHighlights();
    }
    
}