using System;
using System.Collections.Generic;
using UnityEngine;

public class KingPowerManager : MonoBehaviour
{
    public static KingPowerManager Instance { get; private set; }

    public event Action<bool, PieceType> OnPowerGained;
    public event Action<bool, PieceType> OnPowerLost;
    public event Action<bool> OnExtraLifeGained;

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
        ChessPieceManager.Instance.OnPawnPromoted += HandlePawnPromoted;
        ResetState();
    }

    private void OnDestroy()
    {
        if (PieceCaptureManager.Instance != null)
            PieceCaptureManager.Instance.OnPieceCaptured -= HandlePieceCaptured;
        if (ChessPieceManager.Instance != null)
            ChessPieceManager.Instance.OnPawnPromoted -= HandlePawnPromoted;
    }

    private void HandlePieceCaptured(ChessPiece capturedPiece)
    {
        bool isWhitePiece = capturedPiece.IsWhite;
        var pieceCounts = isWhitePiece ? _whitePieceCounts : _blackPieceCounts;
        var kingPowers = isWhitePiece ? _blackKingPowers : _whiteKingPowers;
        var extraLifeTarget = isWhitePiece ? ref _blackKingHasExtraLife : ref _blackKingHasExtraLife;
        var capturedList = isWhitePiece ? PieceCaptureManager.Instance.BlackCapturedPieces : PieceCaptureManager.Instance.WhiteCapturedPieces;

        if (pieceCounts.ContainsKey(capturedPiece.Type))
        {
            pieceCounts[capturedPiece.Type]--;
            if (pieceCounts[capturedPiece.Type] == 0)
            {
                if (capturedPiece.Type != PieceType.Pawn && capturedPiece.Type != PieceType.Queen)
                {
                    if (kingPowers.Add(capturedPiece.Type))
                    {
                        OnPowerGained?.Invoke(!isWhitePiece, capturedPiece.Type);
                    }
                }
            }
        }
        if (!extraLifeTarget && capturedList.Count >= 8)
        {
            extraLifeTarget = true;
            OnExtraLifeGained?.Invoke(!isWhitePiece);
        }
    }

    private void HandlePawnPromoted(ChessPiece pawn, PieceType promotedToType)
    {
        if (promotedToType == PieceType.Queen) return;
        bool isWhitePawn = pawn.IsWhite;
        var pieceCounts = isWhitePawn ? _whitePieceCounts : _blackPieceCounts;
        var kingPowers = isWhitePawn ? _whiteKingPowers : _blackKingPowers;

        if (kingPowers.Remove(promotedToType))
        {
            OnPowerLost?.Invoke(isWhitePawn, promotedToType);
        }
        pieceCounts[promotedToType]++;
    }

    public bool KingHasPower(bool isWhiteKing, PieceType powerType) => (isWhiteKing ? _whiteKingPowers : _blackKingPowers).Contains(powerType);
    public bool KingHasExtraLife(bool isWhiteKing) => isWhiteKing ? _whiteKingHasExtraLife : _blackKingHasExtraLife;
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
        _whitePieceCounts = new Dictionary<PieceType, int> { { PieceType.Rook, 2 }, { PieceType.Knight, 2 }, { PieceType.Bishop, 2 }, { PieceType.Queen, 1 } };
        _blackPieceCounts = new Dictionary<PieceType, int> { { PieceType.Rook, 2 }, { PieceType.Knight, 2 }, { PieceType.Bishop, 2 }, { PieceType.Queen, 1 } };
    }
}