using UnityEngine;

public class ChessPieceManager : MonoBehaviour
{

    [Header("Piece Prefabs")]

    [SerializeField] private GameObject whitePawnPrefab;
    [SerializeField] private GameObject whiteRookPrefab;
    [SerializeField] private GameObject whiteKnightPrefab;
    [SerializeField] private GameObject whiteBishopPrefab;
    [SerializeField] private GameObject whiteQueenPrefab;
    [SerializeField] private GameObject whiteKingPrefab;

    [SerializeField] private GameObject blackPawnPrefab;
    [SerializeField] private GameObject blackRookPrefab;
    [SerializeField] private GameObject blackKnightPrefab;
    [SerializeField] private GameObject blackBishopPrefab;
    [SerializeField] private GameObject blackQueenPrefab;
    [SerializeField] private GameObject blackKingPrefab;


    [Header("Dependencies")]
    [SerializeField] private Chessboard chessboard;

    void Start()
    {
        SpawnAllPieces();
    }

    void SpawnAllPieces()
    {
        // Spawn White Pieces
        SpawnPiece(whiteRookPrefab,   new Vector2Int(0, 0), true);
        SpawnPiece(whiteKnightPrefab, new Vector2Int(1, 0), true);
        SpawnPiece(whiteBishopPrefab, new Vector2Int(2, 0), true);
        SpawnPiece(whiteQueenPrefab,  new Vector2Int(3, 0), true);
        SpawnPiece(whiteKingPrefab,   new Vector2Int(4, 0), true);
        SpawnPiece(whiteBishopPrefab, new Vector2Int(5, 0), true);
        SpawnPiece(whiteKnightPrefab, new Vector2Int(6, 0), true);
        SpawnPiece(whiteRookPrefab,   new Vector2Int(7, 0), true);
        for (int i = 0; i < 8; i++)
        {
            SpawnPiece(whitePawnPrefab, new Vector2Int(i, 1), true);
        }

        // Spawn Black Pieces
        SpawnPiece(blackRookPrefab,   new Vector2Int(0, 7), false);
        SpawnPiece(blackKnightPrefab, new Vector2Int(1, 7), false);
        SpawnPiece(blackBishopPrefab, new Vector2Int(2, 7), false);
        SpawnPiece(blackQueenPrefab,  new Vector2Int(3, 7), false);
        SpawnPiece(blackKingPrefab,   new Vector2Int(4, 7), false);
        SpawnPiece(blackBishopPrefab, new Vector2Int(5, 7), false);
        SpawnPiece(blackKnightPrefab, new Vector2Int(6, 7), false);
        SpawnPiece(blackRookPrefab,   new Vector2Int(7, 7), false);
        for (int i = 0; i < 8; i++)
        {
            SpawnPiece(blackPawnPrefab, new Vector2Int(i, 6), false);
        }
    }

    void SpawnPiece(GameObject prefab, Vector2Int position, bool isWhite)
    {
        GameObject pieceObject = Instantiate(prefab, chessboard.transform);
        
        pieceObject.transform.position = chessboard.GetWorldPosition(position);

        ChessPiece piece = pieceObject.GetComponent<ChessPiece>();
        // Pass the color to the Initialize method
        piece.Initialize(isWhite, position); 

        chessboard.SetPiece(piece, position);
    }
}