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
        if (capturedPiece.Type == PieceType.King)
        {
            return; // Exit the method immediately.
        }
        // -----------------------

        bool isWhitePiece = capturedPiece.IsWhite;
        // --- Let's be very explicit for clarity ---

        if (capturedPiece.IsWhite)
        {
            // A WHITE piece was captured.
            _whitePieceCounts[capturedPiece.Type]--;

            // Check if this was the last white piece of its type.
            if (_whitePieceCounts[capturedPiece.Type] == 0)
            {
                // If so, the WHITE king gains a power.
                if (capturedPiece.Type != PieceType.Pawn && capturedPiece.Type != PieceType.Queen)
                {
                    if (_whiteKingPowers.Add(capturedPiece.Type))
                    {
                        Debug.Log($"<color=yellow>White King has gained the power of the {capturedPiece.Type}!</color>");
                        OnPowerGained?.Invoke(true, capturedPiece.Type); // isWhiteKing = true
                    }
                }
            }

            // Check if the BLACK player (who made the capture) has now captured enough pieces to get an extra life.
            // We check the list of pieces Black has captured (which are White pieces).
            if (!_blackKingHasExtraLife && PieceCaptureManager.Instance.WhiteCapturedPieces.Count >= GameManager.Instance.CurrentSettings.CapturesForExtraLife)
            {
                _blackKingHasExtraLife = true;
                Debug.Log($"<color=green>Black King has gained an Extra Life!</color>");
                OnExtraLifeGained?.Invoke(false); // isWhiteKing = false
            }
        }
        else // The captured piece is Black
        {
            // A BLACK piece was captured.
            _blackPieceCounts[capturedPiece.Type]--;

            // Check if this was the last black piece of its type.
            if (_blackPieceCounts[capturedPiece.Type] == 0)
            {
                // If so, the BLACK king gains a power.
                if (capturedPiece.Type != PieceType.Pawn && capturedPiece.Type != PieceType.Queen)
                {
                    if (_blackKingPowers.Add(capturedPiece.Type))
                    {
                        Debug.Log($"<color=yellow>Black King has gained the power of the {capturedPiece.Type}!</color>");
                        OnPowerGained?.Invoke(false, capturedPiece.Type); // isWhiteKing = false
                    }
                }
            }

            // Check if the WHITE player (who made the capture) has now captured enough pieces to get an extra life.
            // We check the list of pieces White has captured (which are Black pieces).
            if (!_whiteKingHasExtraLife && PieceCaptureManager.Instance.BlackCapturedPieces.Count >= GameManager.Instance.CurrentSettings.CapturesForExtraLife)
            {
                _whiteKingHasExtraLife = true;
                Debug.Log($"<color=green>White King has gained an Extra Life!</color>");
                OnExtraLifeGained?.Invoke(true); // isWhiteKing = true
            }
        }
    }

    private void HandlePawnPromoted(ChessPiece pawn, PieceType promotedToType)
    {
        if (promotedToType == PieceType.Queen || promotedToType == PieceType.Pawn) return;

        // --- Let's be very explicit for clarity ---

        if (pawn.IsWhite)
        {
            // A WHITE pawn was promoted.
            // This increases the count of white pieces of that type.
            _whitePieceCounts[promotedToType]++;

            // If the WHITE king had this power, it is now lost.
            if (_whiteKingPowers.Remove(promotedToType))
            {
                OnPowerLost?.Invoke(true, promotedToType); // isWhiteKing = true
            }
        }
        else // The pawn is Black
        {
            // A BLACK pawn was promoted.
            // This increases the count of black pieces of that type.
            _blackPieceCounts[promotedToType]++;

            // If the BLACK king had this power, it is now lost.
            if (_blackKingPowers.Remove(promotedToType))
            {
                OnPowerLost?.Invoke(false, promotedToType); // isWhiteKing = false
            }
        }
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

        // --- THIS IS THE FIX ---
        // Initialize the piece counts for a standard game, now including Pawns.
        _whitePieceCounts = new Dictionary<PieceType, int>
        {
            { PieceType.Pawn, 8 }, // Added this line
            { PieceType.Rook, 2 },
            { PieceType.Knight, 2 },
            { PieceType.Bishop, 2 },
            { PieceType.Queen, 1 }
        };
        _blackPieceCounts = new Dictionary<PieceType, int>
        {
            { PieceType.Pawn, 8 }, // Added this line
            { PieceType.Rook, 2 },
            { PieceType.Knight, 2 },
            { PieceType.Bishop, 2 },
            { PieceType.Queen, 1 }
        };
        // -----------------------
    }
}