using System;
using System.Collections.Generic;
using UnityEngine;

public class PieceCaptureManager : MonoBehaviour
{
    public static PieceCaptureManager Instance { get; private set; }

    private List<ChessPiece> _whiteCapturedPieces = new List<ChessPiece>();
    private List<ChessPiece> _blackCapturedPieces = new List<ChessPiece>();

    public event Action<ChessPiece, List<ChessPiece>> OnPieceCaptured;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); }
        else { Instance = this; }
    }

    public void CapturePiece(ChessPiece piece)
    {
        if (piece == null) return;

        List<ChessPiece> relevantList;
        if (piece.IsWhite)
        {
            _blackCapturedPieces.Add(piece);
            relevantList = _blackCapturedPieces;
        }
        else
        {
            _whiteCapturedPieces.Add(piece);
            relevantList = _whiteCapturedPieces;
        }

        OnPieceCaptured?.Invoke(piece, relevantList);
        piece.gameObject.SetActive(false);
    }

    public void ClearCapturedLists()
    {
        _whiteCapturedPieces.Clear();
        _blackCapturedPieces.Clear();
    }
}