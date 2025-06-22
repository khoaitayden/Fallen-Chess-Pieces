using UnityEngine;

public class ChessPieceManager : MonoBehaviour
{
    [Header("Piece Prefabs")]
    [SerializeField] private GameObject whitePawnPrefab;

    [Header("Dependencies")]
    [SerializeField] private Chessboard chessboard;

    void Start()
    {
        SpawnPiece(whitePawnPrefab, new Vector2Int(1, 1));
    }

    void SpawnPiece(GameObject prefab, Vector2Int position)
    {
        GameObject pieceObject = Instantiate(prefab, chessboard.transform);
        
        pieceObject.transform.position = chessboard.GetWorldPosition(position);

        ChessPiece piece = pieceObject.GetComponent<ChessPiece>();
        piece.Initialize(true, position);

        chessboard.SetPiece(piece, position);
    }
}