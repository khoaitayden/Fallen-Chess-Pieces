using System.Collections.Generic;
using UnityEngine;

public class KingPowerManager : MonoBehaviour
{
    public static KingPowerManager Instance { get; private set; }

    private HashSet<PieceType> _whiteKingPowers = new HashSet<PieceType>();
    private HashSet<PieceType> _blackKingPowers = new HashSet<PieceType>();

    private bool _whiteKingHasExtraLife = false;
    private bool _blackKingHasExtraLife = false;

    private Dictionary<PieceType, int> _whitePieceCounts = new Dictionary<PieceType, int>();
    private Dictionary<PieceType, int> _blackPieceCounts = new Dictionary<PieceType, int>();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); }
        else { Instance = this; }
    }

    private void Start()
    {
        PieceCaptureManager.Instance.OnPieceCaptured += HandlePieceCaptured;
        ResetState();
    }

    private void OnDestroy()
    {
        if (PieceCaptureManager.Instance != null)
        {
            PieceCaptureManager.Instance.OnPieceCaptured -= HandlePieceCaptured;
        }
    }

    private void HandlePieceCaptured(ChessPiece capturedPiece)
    {
        bool isWhitePiece = capturedPiece.IsWhite;
        var pieceCounts = isWhitePiece ? _whitePieceCounts : _blackPieceCounts;
        var kingPowers = isWhitePiece ? _blackKingPowers : _whiteKingPowers; // The OPPOSITE king gains power
        var extraLifeTarget = isWhitePiece ? ref _blackKingHasExtraLife : ref _whiteKingHasExtraLife;
        var capturedList = isWhitePiece ? PieceCaptureManager.Instance.BlackCapturedPieces : PieceCaptureManager.Instance.WhiteCapturedPieces;

        if (pieceCounts.ContainsKey(capturedPiece.Type))
        {
            pieceCounts[capturedPiece.Type]--;

            if (pieceCounts[capturedPiece.Type] == 0)
            {
                if (capturedPiece.Type != PieceType.Pawn && capturedPiece.Type != PieceType.Queen)
                {
                    kingPowers.Add(capturedPiece.Type);
                    Debug.Log($"<color=yellow>{(isWhitePiece ? "Black" : "White")} King has gained the power of the {capturedPiece.Type}!</color>");
                }
            }
        }

        if (!extraLifeTarget && capturedList.Count >= 8)
        {
            extraLifeTarget = true;
            Debug.Log($"<color=green>{(isWhitePiece ? "Black" : "White")} King has gained an Extra Life!</color>");
        }
    }

    public bool KingHasPower(bool isWhiteKing, PieceType powerType)
    {
        var powers = isWhiteKing ? _whiteKingPowers : _blackKingPowers;
        return powers.Contains(powerType);
    }
    
    public bool KingHasExtraLife(bool isWhiteKing)
    {
        return isWhiteKing ? _whiteKingHasExtraLife : _blackKingHasExtraLife;
    }
    
    public void ConsumeExtraLife(bool isWhiteKing)
    {
        if (isWhiteKing) _whiteKingHasExtraLife = false;
        else _blackKingHasExtraLife = false;
    }

    public void ResetState()
    {
        _whiteKingPowers.Clear();
        _blackKingPowers.Clear();
        _whiteKingHasExtraLife = false;
        _blackKingHasExtraLife = false;

        _whitePieceCounts = new Dictionary<PieceType, int>
        {
            { PieceType.Rook, 2 }, { PieceType.Knight, 2 }, { PieceType.Bishop, 2 }, { PieceType.Queen, 1 }
        };
        _blackPieceCounts = new Dictionary<PieceType, int>
        {
            { PieceType.Rook, 2 }, { PieceType.Knight, 2 }, { PieceType.Bishop, 2 }, { PieceType.Queen, 1 }
        };
    }
}