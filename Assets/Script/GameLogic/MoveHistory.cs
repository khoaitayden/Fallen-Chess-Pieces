using System;
using System.Collections.Generic;
using UnityEngine;

public class MoveHistory : MonoBehaviour
{
    public static MoveHistory Instance { get; private set; }

    // A private list to store the actual move data.
    private readonly List<MoveData> _moves = new List<MoveData>();

    // An event that the UI can subscribe to. It fires whenever a move is added.
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

    // Adds a move to the history and notifies any listeners.
    public void AddMove(MoveData move)
    {
        _moves.Add(move);
        OnMoveAdded?.Invoke(move); // Fire the event
    }

    // Clears the history, for starting a new game.
    public void ClearHistory()
    {
        _moves.Clear();
    }
}