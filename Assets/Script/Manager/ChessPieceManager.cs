// In Scripts/Manager/ChessPieceManager.cs
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChessPieceManager : MonoBehaviour
{
    public static ChessPieceManager Instance { get; private set; }
    public event Action<ChessPiece, PieceType> OnPawnPromoted;

    [Header("Standard Piece Prefabs")]
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

    [Header("Combination Piece Prefabs")]
    [SerializeField] private GameObject whiteKnightRookPrefab;
    [SerializeField] private GameObject whiteKnightBishopPrefab;
    [SerializeField] private GameObject whiteRookBishopPrefab;
    [SerializeField] private GameObject whiteKnightBishopRookPrefab;
    [SerializeField] private GameObject blackKnightRookPrefab;
    [SerializeField] private GameObject blackKnightBishopPrefab;
    [SerializeField] private GameObject blackRookBishopPrefab;
    [SerializeField] private GameObject blackKnightBishopRookPrefab;

    [Header("Dependencies")]
    [SerializeField] private Chessboard chessboard;

    private Dictionary<PieceType, GameObject> _whitePrefabs = new Dictionary<PieceType, GameObject>();
    private Dictionary<PieceType, GameObject> _blackPrefabs = new Dictionary<PieceType, GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); }
        else { Instance = this; }
        InitializePrefabDictionaries();
    }

    private void InitializePrefabDictionaries()
    {
        // Standard White
        _whitePrefabs[PieceType.Pawn] = whitePawnPrefab;
        _whitePrefabs[PieceType.Rook] = whiteRookPrefab;
        _whitePrefabs[PieceType.Knight] = whiteKnightPrefab;
        _whitePrefabs[PieceType.Bishop] = whiteBishopPrefab;
        _whitePrefabs[PieceType.Queen] = whiteQueenPrefab;
        _whitePrefabs[PieceType.King] = whiteKingPrefab;
        // Combination White
        _whitePrefabs[PieceType.KnightRook] = whiteKnightRookPrefab;
        _whitePrefabs[PieceType.KnightBishop] = whiteKnightBishopPrefab;
        _whitePrefabs[PieceType.RookBishop] = whiteRookBishopPrefab;
        _whitePrefabs[PieceType.KnightBishopRook] = whiteKnightBishopRookPrefab;

        // Standard Black
        _blackPrefabs[PieceType.Pawn] = blackPawnPrefab;
        _blackPrefabs[PieceType.Rook] = blackRookPrefab;
        _blackPrefabs[PieceType.Knight] = blackKnightPrefab;
        _blackPrefabs[PieceType.Bishop] = blackBishopPrefab;
        _blackPrefabs[PieceType.Queen] = blackQueenPrefab;
        _blackPrefabs[PieceType.King] = blackKingPrefab;
        // Combination Black
        _blackPrefabs[PieceType.KnightRook] = blackKnightRookPrefab;
        _blackPrefabs[PieceType.KnightBishop] = blackKnightBishopPrefab;
        _blackPrefabs[PieceType.RookBishop] = blackRookBishopPrefab;
        _blackPrefabs[PieceType.KnightBishopRook] = blackKnightBishopRookPrefab;
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
        OnPawnPromoted?.Invoke(pawn, newType);
    }

    private GameObject GetPrefabForPiece(PieceType type, bool isWhite)
    {
        var prefabs = isWhite ? _whitePrefabs : _blackPrefabs;
        return prefabs.ContainsKey(type) ? prefabs[type] : null;
    }

    private void SpawnPiece(PieceType type, Vector2Int position, bool isWhite, bool hasMoved = false)
    {
        GameObject prefab = GetPrefabForPiece(type, isWhite);
        if (prefab == null) return;
        GameObject pieceObject = Instantiate(prefab, chessboard.transform);
        pieceObject.transform.position = chessboard.GetWorldPosition(position);
        ChessPiece piece = pieceObject.GetComponent<ChessPiece>();
        piece.Initialize(isWhite, position);
        piece._hasMoved = hasMoved; // Preserve the moved state
        chessboard.SetPiece(piece, position);
    }

    public Sprite GetSpriteForPiece(PieceType type, bool isWhite)
    {
        GameObject prefab = GetPrefabForPiece(type, isWhite);
        return prefab != null ? prefab.GetComponent<SpriteRenderer>().sprite : null;
    }
    
    // This method is called by the PowerManager when a piece's powers change.
    public void UpdatePieceAppearance(ChessPiece pieceToUpdate)
    {
        if (pieceToUpdate == null) return;

        Vector2Int position = pieceToUpdate._boardPosition;
        bool isWhite = pieceToUpdate.IsWhite;

        List<PieceType> powers = PowerManager.Instance.GetPowersForPiece(position);
        
        // Add the piece's original power to the list for easier checking.
        if (pieceToUpdate.Type == PieceType.Rook || pieceToUpdate.Type == PieceType.Knight || pieceToUpdate.Type == PieceType.Bishop)
        {
            if (!powers.Contains(pieceToUpdate.Type))
            {
                powers.Add(pieceToUpdate.Type);
            }
        }

        PieceType newType = pieceToUpdate.Type;

        bool hasKnight = powers.Contains(PieceType.Knight);
        bool hasRook = powers.Contains(PieceType.Rook);
        bool hasBishop = powers.Contains(PieceType.Bishop);

        if (hasKnight && hasRook && hasBishop) newType = PieceType.KnightBishopRook;
        else if (hasKnight && hasRook)         newType = PieceType.KnightRook;
        else if (hasKnight && hasBishop)       newType = PieceType.KnightBishop;
        else if (hasRook && hasBishop)         newType = PieceType.RookBishop;
        // No need for single-power fallbacks, as we want to keep the original piece type if only one power exists.

        if (newType != pieceToUpdate.Type)
        {
            Debug.Log($"Swapping piece at {position} from {pieceToUpdate.Type} to {newType}");
            
            chessboard.SetPiece(null, position);
            Destroy(pieceToUpdate.gameObject);

            SpawnPiece(newType, position, isWhite);
        }
    }
}