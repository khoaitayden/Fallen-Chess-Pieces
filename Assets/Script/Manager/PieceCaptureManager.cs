using System;
using System.Collections.Generic;
using UnityEngine;

public class PieceCaptureManager : MonoBehaviour
{
    public static PieceCaptureManager Instance { get; private set; }

    public List<ChessPiece> WhiteCapturedPieces { get; private set; } = new List<ChessPiece>();
    public List<ChessPiece> BlackCapturedPieces { get; private set; } = new List<ChessPiece>();

    public event Action<ChessPiece> OnPieceCaptured;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void CapturePiece(ChessPiece piece)
    {
        if (piece == null) return;

        if (piece.IsWhite)
        {
            BlackCapturedPieces.Add(piece);
        }
        else
        {
            WhiteCapturedPieces.Add(piece);
        }

        OnPieceCaptured?.Invoke(piece);


        piece.gameObject.SetActive(false);
    }

    public void ClearCapturedLists()
    {
        WhiteCapturedPieces.Clear();
        BlackCapturedPieces.Clear();
    }
}