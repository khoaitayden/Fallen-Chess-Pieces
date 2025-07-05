using System;
using System.Collections.Generic;
using UnityEngine;

public class MoveHistory : MonoBehaviour
{
    public static MoveHistory Instance { get; private set; }

    private readonly List<MoveData> _moves = new List<MoveData>();

    public event Action<MoveData> OnMoveAdded;

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

    public void AddMove(MoveData move)
    {
        _moves.Add(move);
        OnMoveAdded?.Invoke(move);
    }

    public void ClearHistory()
    {
        _moves.Clear();
    }
}