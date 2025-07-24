using System;
using System.Collections.Generic;
using UnityEngine;

public class KingPowerManager : MonoBehaviour
{
    public static KingPowerManager Instance { get; private set; }
    public event Action<bool> OnExtraLifeGained;

    private bool _whiteKingHasExtraLife = false;
    private bool _blackKingHasExtraLife = false;

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
            PieceCaptureManager.Instance.OnPieceCaptured -= HandlePieceCaptured;
    }

    // --- THIS IS THE FIX ---
    // The method now correctly accepts both the piece and the list.
    private void HandlePieceCaptured(ChessPiece capturedPiece, List<ChessPiece> capturedList)
    {
        if (capturedPiece.Type == PieceType.King) return;

        bool isWhitePiece = capturedPiece.IsWhite;
        bool capturingPlayerIsWhite = !isWhitePiece;
        int capturesNeeded = GameManager.Instance.CurrentSettings.CapturesForExtraLife;
        
        // We only check the extra life for the player who made the capture.
        ref bool extraLifeTarget = ref (capturingPlayerIsWhite ? ref _whiteKingHasExtraLife : ref _blackKingHasExtraLife);

        if (!extraLifeTarget && capturesNeeded > 0 && capturedList.Count >= capturesNeeded)
        {
            extraLifeTarget = true;
            OnExtraLifeGained?.Invoke(capturingPlayerIsWhite);
        }
    }
    // -----------------------

    public bool KingHasExtraLife(bool isWhiteKing) => isWhiteKing ? _whiteKingHasExtraLife : _blackKingHasExtraLife;
    
    public void ConsumeExtraLife(bool isWhiteKing)
    {
        if (isWhiteKing) _whiteKingHasExtraLife = false;
        else _blackKingHasExtraLife = false;
    }

    public void ResetState()
    {
        _whiteKingHasExtraLife = false;
        _blackKingHasExtraLife = false;
    }
}