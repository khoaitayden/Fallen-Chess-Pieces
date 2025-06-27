using UnityEngine;

public class Chessboard : MonoBehaviour
{
    [Header("Board Visuals")]
    [SerializeField] private GameObject whiteSquarePrefab;
    [SerializeField] private GameObject blackSquarePrefab;
    [SerializeField] private float squareSize = 1f;
    [SerializeField] private Vector3 boardOffset = Vector3.zero;

    private BoardSquare[,] _boardSquares = new BoardSquare[Constants.BOARD_SIZE, Constants.BOARD_SIZE];

    private ChessPiece[,] _pieces = new ChessPiece[Constants.BOARD_SIZE, Constants.BOARD_SIZE];

    void Awake()
    {
        GenerateBoard();
    }

    void GenerateBoard()
    {
        foreach (Transform child in transform) { Destroy(child.gameObject); }

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
        return new Vector3(boardPos.x * squareSize, boardPos.y * squareSize, -1f) + boardOffset;
    }

    public ChessPiece GetPieceAt(Vector2Int position)
    {
        if (position.x < 0 || position.x >= Constants.BOARD_SIZE || position.y < 0 || position.y >= Constants.BOARD_SIZE)
            return null;

        return _pieces[position.x, position.y];
    }

    public void SetPiece(ChessPiece piece, Vector2Int position)
    {
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
        Vector2Int oldPosition = piece._boardPosition;

        //Check for Castling
        if (piece.Type == PieceType.King && Mathf.Abs(newPosition.x - oldPosition.x) == 2)
        {
            // The HandleCastle method already moves both pieces and updates the array.
            // We can exit the method immediately after.
            HandleCastle(piece, oldPosition, newPosition);
            return;
        }

        //Check for En Passant capture
        if (piece.Type == PieceType.Pawn && newPosition == TurnManager.Instance.EnPassantTargetSquare)
        {
            int direction = piece.IsWhite ? -1 : 1;
            Vector2Int capturedPawnPos = new Vector2Int(newPosition.x, newPosition.y + direction);
            ChessPiece capturedPawn = GetPieceAt(capturedPawnPos);
            if (capturedPawn != null)
            {
                PieceCaptureManager.Instance.CapturePiece(capturedPawn); 
                _pieces[capturedPawnPos.x, capturedPawnPos.y] = null; 
            }
        }

        ChessPiece capturedPiece = GetPieceAt(newPosition);
        if (capturedPiece != null)
        {
            PieceCaptureManager.Instance.CapturePiece(capturedPiece);
        }

        _pieces[oldPosition.x, oldPosition.y] = null;
        _pieces[newPosition.x, newPosition.y] = piece;

        Vector3 worldPosition = GetWorldPosition(newPosition);
        piece.MoveTo(newPosition, worldPosition);
    }

    private void HandleCastle(ChessPiece king, Vector2Int oldKingPos, Vector2Int newKingPos)
    {
        _pieces[oldKingPos.x, oldKingPos.y] = null;
        _pieces[newKingPos.x, newKingPos.y] = king;
        king.MoveTo(newKingPos, GetWorldPosition(newKingPos));

        Vector2Int rookOldPos, rookNewPos;
        if (newKingPos.x > oldKingPos.x) 
        {
            rookOldPos = new Vector2Int(7, oldKingPos.y);
            rookNewPos = new Vector2Int(newKingPos.x - 1, oldKingPos.y);
        }
        else 
        {
            rookOldPos = new Vector2Int(0, oldKingPos.y);
            rookNewPos = new Vector2Int(newKingPos.x + 1, oldKingPos.y);
        }

        ChessPiece rook = GetPieceAt(rookOldPos);
        if (rook != null)
        {
            _pieces[rookOldPos.x, rookOldPos.y] = null;
            _pieces[rookNewPos.x, rookNewPos.y] = rook;
            rook.MoveTo(rookNewPos, GetWorldPosition(rookNewPos));
        }
        else
        {
            Debug.LogError($"Castling failed: Rook not found at {rookOldPos}");
        }
    }

    public ChessPiece SimulateMove(ChessPiece piece, Vector2Int newPosition)
    {
        Vector2Int oldPosition = piece._boardPosition;
        ChessPiece capturedPiece = GetPieceAt(newPosition);

        _pieces[newPosition.x, newPosition.y] = piece;
        _pieces[oldPosition.x, oldPosition.y] = null;
        
        piece._boardPosition = newPosition;

        return capturedPiece;
    }

    public void UndoSimulatedMove(ChessPiece piece, Vector2Int originalPosition, ChessPiece capturedPiece)
    {
        Vector2Int currentPosition = piece._boardPosition;

        _pieces[originalPosition.x, originalPosition.y] = piece;
        _pieces[currentPosition.x, currentPosition.y] = capturedPiece; 

        piece._boardPosition = originalPosition;
    }
}