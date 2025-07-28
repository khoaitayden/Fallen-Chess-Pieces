// In Scripts/Manager/PowerManager.cs
using System;
using System.Collections.Generic;
using UnityEngine;

public class PowerManager : MonoBehaviour
{
    public static PowerManager Instance { get; private set; }

    public event Action<bool, PieceType> OnPowerTransferRequired; 
    public event Action<bool> OnExtraLifeGained; 

    private Dictionary<Vector2Int, List<PieceType>> _piecePowers = new Dictionary<Vector2Int, List<PieceType>>();
    private int _whiteExtraLifeTokens = 0;
    private int _blackExtraLifeTokens = 0;
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

    private void HandlePieceCaptured(ChessPiece capturedPiece, List<ChessPiece> fullCapturedList)
    {
        
        if (capturedPiece.Type == PieceType.King) return;

        bool isWhitePiece = capturedPiece.IsWhite;
        var pieceCounts = isWhitePiece ? _whitePieceCounts : _blackPieceCounts;

        if (pieceCounts.ContainsKey(capturedPiece.Type))
        {
            pieceCounts[capturedPiece.Type]--;

            if (pieceCounts[capturedPiece.Type] == 0)
            {
                if (capturedPiece.Type == PieceType.Rook || capturedPiece.Type == PieceType.Knight || capturedPiece.Type == PieceType.Bishop)
                {
                    // Announce that the player who lost the piece needs to make a choice.
                    OnPowerTransferRequired?.Invoke(isWhitePiece, capturedPiece.Type);
                }
            }
        }

        bool capturingPlayerIsWhite = !isWhitePiece;
        int capturesNeeded = GameManager.Instance.CurrentSettings.CapturesForExtraLife;

        if (capturesNeeded > 0 && fullCapturedList.Count > 0 && fullCapturedList.Count % capturesNeeded == 0)
        {
            if (capturingPlayerIsWhite) _whiteExtraLifeTokens++;
            else _blackExtraLifeTokens++;

            OnExtraLifeGained?.Invoke(capturingPlayerIsWhite);
        }
    }


    public void GrantPower(ChessPiece targetPiece, PieceType powerType)
    {
        Vector2Int targetPosition = targetPiece._boardPosition;
        if (!_piecePowers.ContainsKey(targetPosition))
        {
            _piecePowers[targetPosition] = new List<PieceType>();
        }
        if (!_piecePowers[targetPosition].Contains(powerType))
        {
            _piecePowers[targetPosition].Add(powerType);
            Debug.Log($"Power of {powerType} granted to piece at {targetPosition}.");
            ChessPieceManager.Instance.UpdatePieceAppearance(targetPiece);
        }
    }

    public void UpdatePiecePosition(Vector2Int from, Vector2Int to)
    {
        List<PieceType> powers = new List<PieceType>();
        if (_piecePowers.ContainsKey(from))
        {
            powers = _piecePowers[from];
            _piecePowers.Remove(from);
        }
        _piecePowers[to] = powers;
    }

    public List<PieceType> GetPowersForPiece(Vector2Int piecePosition)
    {
        return _piecePowers.ContainsKey(piecePosition) ? _piecePowers[piecePosition] : new List<PieceType>();
    }

    public bool PlayerHasExtraLifeTokens(bool isWhite)
    {
        return isWhite ? _whiteExtraLifeTokens > 0 : _blackExtraLifeTokens > 0;
    }

    public void ConsumeExtraLifeToken(bool isWhite)
    {
        if (isWhite) { if (_whiteExtraLifeTokens > 0) _whiteExtraLifeTokens--; }
        else { if (_blackExtraLifeTokens > 0) _blackExtraLifeTokens--; }
    }

    public void ResetState()
    {
        _piecePowers.Clear();
        _whiteExtraLifeTokens = 0;
        _blackExtraLifeTokens = 0;
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