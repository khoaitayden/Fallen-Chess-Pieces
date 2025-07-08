using System.Collections.Generic;
using UnityEngine;

public class HumanPlayer : Player
{
    private ChessPiece _selectedPiece;
    private List<Vector2Int> _validMoves;
    private Chessboard _chessboard;

    //online
    private NetworkMoveRelay _moveRelay;

    public HumanPlayer(bool isWhite) : base(isWhite, PlayerType.Human)
    {
        _chessboard = Object.FindObjectOfType<Chessboard>();
        _moveRelay = null;
    }

    public HumanPlayer(bool isWhite, NetworkMoveRelay moveRelay) : base(isWhite, PlayerType.Human)
    {
        _chessboard = Object.FindObjectOfType<Chessboard>();
        _moveRelay = moveRelay;
    }

    public override void OnTurnStart()
    {
        InputController.Instance.OnBoardClick += HandleBoardClick;
    }

    private void HandleBoardClick(Vector2Int position)
    {
        if (GameManager.Instance.CurrentState != GameState.Playing) return;

        if (_selectedPiece != null)
        {
            if (_validMoves != null && _validMoves.Contains(position))
            {
                MakeMove(position);
                return;
            }
            ChessPiece pieceAtPos = _chessboard.GetPieceAt(position);
            if (pieceAtPos != null && pieceAtPos.IsWhite == this.IsWhite)
            {
                DeselectPiece();
                SelectPiece(pieceAtPos);
                return;
            }
            DeselectPiece();
        }
        else
        {
            AttemptSelection(position);
        }
    }

    private void MakeMove(Vector2Int toPosition)
    {
        InputController.Instance.OnBoardClick -= HandleBoardClick;

        // --- NEW UNIFIED LOGIC ---
        if (GameManager.Instance.CurrentGameMode == GameMode.Online && _moveRelay != null)
        {
        InputController.Instance.OnBoardClick -= HandleBoardClick; // Stop listening
        _moveRelay.CmdSendMove(_selectedPiece._boardPosition, toPosition);
        DeselectPiece();
        }
        else
        {
            Vector2Int oldPosition = _selectedPiece._boardPosition;
            _chessboard.MovePiece(_selectedPiece, toPosition);
            
            if (_selectedPiece.Type == PieceType.Pawn && (toPosition.y == 0 || toPosition.y == 7))
            {
                DeselectPiece();
                return; 
            }

            TurnManager.Instance.SetEnPassantTarget(_selectedPiece, oldPosition, toPosition);
            TurnManager.Instance.SwitchTurn();

            string notation = MoveConverter.ToDescriptiveNotation(_selectedPiece, toPosition);
            MoveData move = new MoveData(_selectedPiece.Type, oldPosition, toPosition, notation);
            MoveHistory.Instance.AddMove(move);

            DeselectPiece();
            GameManager.Instance.CheckForGameEnd();

            if (GameManager.Instance.CurrentState == GameState.Playing)
            {
                GameManager.Instance.NotifyCurrentPlayer();
            }
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