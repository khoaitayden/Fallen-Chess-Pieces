using UnityEngine;

public class Chessboard : MonoBehaviour
{    public static Chessboard Instance { get; private set; }

    [Header("Board Visuals")]
    [SerializeField] private GameObject whiteSquarePrefab;
    [SerializeField] private GameObject blackSquarePrefab;
    [SerializeField] private float squareSize = 1f;
    [SerializeField] private Vector3 boardOffset = Vector3.zero;

    private BoardSquare[,] _boardSquares = new BoardSquare[Constants.BOARD_SIZE, Constants.BOARD_SIZE];
    private ChessPiece[,] _pieces = new ChessPiece[Constants.BOARD_SIZE, Constants.BOARD_SIZE];

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        GenerateBoard();
    }

    public void GenerateBoard()
    {
        foreach (Transform child in transform) { Destroy(child.gameObject); }
        _pieces = new ChessPiece[Constants.BOARD_SIZE, Constants.BOARD_SIZE];

        for (int row = 0; row < Constants.BOARD_SIZE; row++)
        {
            for (int col = 0; col < Constants.BOARD_SIZE; col++)
            {
                bool isWhite = (row + col) % 2 != 0;
                GameObject squarePrefab = isWhite ? whiteSquarePrefab : blackSquarePrefab;
                GameObject squareInstance = Instantiate(squarePrefab, transform);
                Vector3 localPosition = new Vector3(col * squareSize, row * squareSize, 0f) + boardOffset;
                squareInstance.transform.localPosition = localPosition;
                squareInstance.transform.localScale = Vector3.one * squareSize;

                BoardSquare boardSquare = squareInstance.GetComponent<BoardSquare>();
                boardSquare.SetBoardPosition(new Vector2Int(col, row));

                _boardSquares[col, row] = boardSquare;
            }
        }
    }

    public Vector3 GetWorldPosition(Vector2Int boardPos)
    {
        return transform.TransformPoint(GetLocalPosition(boardPos));
    }
        public Vector3 GetLocalPosition(Vector2Int boardPos)
    {
        return new Vector3(boardPos.x * squareSize, boardPos.y * squareSize, -1f);
    }


    public ChessPiece GetPieceAt(Vector2Int position)
    {
        if (position.x < 0 || position.x >= Constants.BOARD_SIZE || position.y < 0 || position.y >= Constants.BOARD_SIZE)
            return null;

        return _pieces[position.x, position.y];
    }

    public void SetPiece(ChessPiece piece, Vector2Int position)
    {
        if (position.x < 0 || position.x >= Constants.BOARD_SIZE || position.y < 0 || position.y >= Constants.BOARD_SIZE)
            return;
            
        _pieces[position.x, position.y] = piece;
    }

    public BoardSquare GetSquareAt(Vector2Int position)
    {
        if (position.x < 0 || position.x >= Constants.BOARD_SIZE || position.y < 0 || position.y >= Constants.BOARD_SIZE)
            return null;

        return _boardSquares[position.x, position.y];
    }

    public void MovePiece(ChessPiece piece, Vector2Int newPosition)
    {
        if (piece.Type == PieceType.Pawn)
        {
            bool isWhitePromotion = piece.IsWhite && newPosition.y == 7;
            bool isBlackPromotion = !piece.IsWhite && newPosition.y == 0;

            if (isWhitePromotion || isBlackPromotion)
            {
                StandardMove(piece, newPosition);
                GameManager.Instance.InitiatePawnPromotion(piece);
                return;
            }
        }

        StandardMove(piece, newPosition);
    }

    public BoardState CreateBoardState()
    {
        BoardState state = new BoardState();
        for (int x = 0; x < Constants.BOARD_SIZE; x++)
        {
            for (int y = 0; y < Constants.BOARD_SIZE; y++)
            {
                ChessPiece piece = _pieces[x, y];
                if (piece != null)
                {
                    state.Pieces[x, y] = new BoardState.PieceData
                    {
                        Type = piece.Type,
                        IsWhite = piece.IsWhite,
                        HasMoved = piece._hasMoved
                    };
                }
            }
        }
        state.EnPassantTargetSquare = TurnManager.Instance.EnPassantTargetSquare;
        return state;
    }
    
    public ChessPiece SimulateMove(ChessPiece piece, Vector2Int newPosition)
    {
        // If ANYONE calls this method, we will know immediately.
        Debug.LogError("FATAL ERROR: The old Chessboard.SimulateMove is still being called! Find the culprit!");
        return null;
    }

    public void UndoSimulatedMove(ChessPiece piece, Vector2Int originalPosition, ChessPiece capturedPiece)
    {
        Debug.LogError("FATAL ERROR: The old Chessboard.UndoSimulatedMove is still being called! Find the culprit!");
    }

    private void StandardMove(ChessPiece piece, Vector2Int newPosition)
    {
        Vector2Int oldPosition = piece._boardPosition;
        bool wasCapture = false;

        if (piece.Type == PieceType.King && Mathf.Abs(newPosition.x - oldPosition.x) == 2)
        {
            HandleCastle(piece, oldPosition, newPosition);
            return; // Castle handles its own sounds and moves
        }

        if (piece.Type == PieceType.Pawn && newPosition == TurnManager.Instance.EnPassantTargetSquare)
        {
            HandleEnPassant(piece, newPosition);
            wasCapture = true; // En passant is a capture
        }

        ChessPiece capturedPiece = GetPieceAt(newPosition);
        if (capturedPiece != null)
        {
            PieceCaptureManager.Instance.CapturePiece(capturedPiece);
            wasCapture = true;
        }

        // Play sound based on whether a capture occurred
        if (wasCapture)
        {
            AudioManager.Instance.PlayCaptureSound();
        }
        else
        {
            AudioManager.Instance.PlayMoveSound();
        }

        _pieces[oldPosition.x, oldPosition.y] = null;
        _pieces[newPosition.x, newPosition.y] = piece;

        Vector3 localPos = GetLocalPosition(newPosition);
        piece.MoveTo(newPosition, localPos);
    }

    private void HandleEnPassant(ChessPiece pawn, Vector2Int targetSquare)
    {
        int direction = pawn.IsWhite ? -1 : 1;
        Vector2Int capturedPawnPos = new Vector2Int(targetSquare.x, targetSquare.y + direction);
        ChessPiece capturedPawn = GetPieceAt(capturedPawnPos);
        if (capturedPawn != null)
        {
            PieceCaptureManager.Instance.CapturePiece(capturedPawn);
            _pieces[capturedPawnPos.x, capturedPawnPos.y] = null;
        }
    }

    private void HandleCastle(ChessPiece king, Vector2Int oldKingPos, Vector2Int newKingPos)
    {
        // --- THIS IS THE ROBUST FIX ---
        // Determine the correct rank (row) based on the king's color.
        int rank = king.IsWhite ? 0 : 7;

        // Move the king first.
        _pieces[oldKingPos.x, oldKingPos.y] = null;
        _pieces[newKingPos.x, newKingPos.y] = king;
        king.MoveTo(newKingPos, GetLocalPosition(newKingPos));

        Vector2Int rookOldPos, rookNewPos;
        if (newKingPos.x > oldKingPos.x) // Kingside
        {
            // The rook is always on file 7 of the correct rank.
            rookOldPos = new Vector2Int(7, rank);
            rookNewPos = new Vector2Int(newKingPos.x - 1, rank);
        }
        else // Queenside
        {
            // The rook is always on file 0 of the correct rank.
            rookOldPos = new Vector2Int(0, rank);
            rookNewPos = new Vector2Int(newKingPos.x + 1, rank);
        }
        // ---------------------------------

        ChessPiece rook = GetPieceAt(rookOldPos);
        if (rook != null)
        {
            _pieces[rookOldPos.x, rookOldPos.y] = null;
            _pieces[rookNewPos.x, rookNewPos.y] = rook;
            rook.MoveTo(rookNewPos, GetLocalPosition(rookNewPos));
            AudioManager.Instance.PlayCastleSound();
        }
        else
        {
            Debug.LogError($"Castling failed: Rook not found at {rookOldPos}");
        }
    }
}