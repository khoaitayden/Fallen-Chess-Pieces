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
}