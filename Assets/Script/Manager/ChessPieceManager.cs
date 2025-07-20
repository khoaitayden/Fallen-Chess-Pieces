using System;
using System.Collections.Generic;
using UnityEngine;

public class ChessPieceManager : MonoBehaviour
{
    public static ChessPieceManager Instance { get; private set; }

    // This event will be fired whenever a pawn is successfully promoted.
    public event Action<ChessPiece, PieceType> OnPawnPromoted;

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

    // Dictionaries for fast, easy access to prefabs by PieceType.
    private Dictionary<PieceType, GameObject> _whitePrefabs = new Dictionary<PieceType, GameObject>();
    private Dictionary<PieceType, GameObject> _blackPrefabs = new Dictionary<PieceType, GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); }
        else { Instance = this; }

        // Populate the dictionaries once at the start.
        InitializePrefabDictionaries();
    }

    private void InitializePrefabDictionaries()
    {
        _whitePrefabs[PieceType.Pawn] = whitePawnPrefab;
        _whitePrefabs[PieceType.Rook] = whiteRookPrefab;
        _whitePrefabs[PieceType.Knight] = whiteKnightPrefab;
        _whitePrefabs[PieceType.Bishop] = whiteBishopPrefab;
        _whitePrefabs[PieceType.Queen] = whiteQueenPrefab;
        _whitePrefabs[PieceType.King] = whiteKingPrefab;

        _blackPrefabs[PieceType.Pawn] = blackPawnPrefab;
        _blackPrefabs[PieceType.Rook] = blackRookPrefab;
        _blackPrefabs[PieceType.Knight] = blackKnightPrefab;
        _blackPrefabs[PieceType.Bishop] = blackBishopPrefab;
        _blackPrefabs[PieceType.Queen] = blackQueenPrefab;
        _blackPrefabs[PieceType.King] = blackKingPrefab;
    }

    public void SpawnAllPieces()
    {
        // Spawn White Pieces
        SpawnPiece(PieceType.Rook, new Vector2Int(0, 0), true);
        SpawnPiece(PieceType.Knight, new Vector2Int(1, 0), true);
        SpawnPiece(PieceType.Bishop, new Vector2Int(2, 0), true);
        SpawnPiece(PieceType.Queen, new Vector2Int(3, 0), true);
        SpawnPiece(PieceType.King, new Vector2Int(4, 0), true);
        SpawnPiece(PieceType.Bishop, new Vector2Int(5, 0), true);
        SpawnPiece(PieceType.Knight, new Vector2Int(6, 0), true);
        SpawnPiece(PieceType.Rook, new Vector2Int(7, 0), true);
        for (int i = 0; i < 8; i++) SpawnPiece(PieceType.Pawn, new Vector2Int(i, 1), true);

        // Spawn Black Pieces
        SpawnPiece(PieceType.Rook, new Vector2Int(0, 7), false);
        SpawnPiece(PieceType.Knight, new Vector2Int(1, 7), false);
        SpawnPiece(PieceType.Bishop, new Vector2Int(2, 7), false);
        SpawnPiece(PieceType.Queen, new Vector2Int(3, 7), false);
        SpawnPiece(PieceType.King, new Vector2Int(4, 7), false);
        SpawnPiece(PieceType.Bishop, new Vector2Int(5, 7), false);
        SpawnPiece(PieceType.Knight, new Vector2Int(6, 7), false);
        SpawnPiece(PieceType.Rook, new Vector2Int(7, 7), false);
        for (int i = 0; i < 8; i++) SpawnPiece(PieceType.Pawn, new Vector2Int(i, 6), false);
    }

    public void PromotePawn(ChessPiece pawn, PieceType newType)
    {
        if (pawn.Type != PieceType.Pawn) return;
        Vector2Int position = pawn._boardPosition;
        bool isWhite = pawn.IsWhite;
        chessboard.SetPiece(null, position);
        Destroy(pawn.gameObject);
        SpawnPiece(newType, position, isWhite);
        
        // Announce that the promotion happened.
        OnPawnPromoted?.Invoke(pawn, newType);
    }

    private GameObject GetPrefabForPiece(PieceType type, bool isWhite)
    {
        var prefabs = isWhite ? _whitePrefabs : _blackPrefabs;
        return prefabs.ContainsKey(type) ? prefabs[type] : null;
    }

    private void SpawnPiece(PieceType type, Vector2Int position, bool isWhite)
    {
        GameObject prefab = GetPrefabForPiece(type, isWhite);
        if (prefab == null) return;
        GameObject pieceObject = Instantiate(prefab, chessboard.transform);
        pieceObject.transform.position = chessboard.GetWorldPosition(position);
        ChessPiece piece = pieceObject.GetComponent<ChessPiece>();
        piece.Initialize(isWhite, position);
        chessboard.SetPiece(piece, position);
    }

    // This is the new, reliable way for the UI to get a sprite.
    public Sprite GetSpriteForPiece(PieceType type, bool isWhite)
    {
        GameObject prefab = GetPrefabForPiece(type, isWhite);
        if (prefab != null && prefab.TryGetComponent(out SpriteRenderer sr))
        {
            return sr.sprite;
        }
        return null;
    }
}