using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    public bool IsWhiteTurn { get; private set; }

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

    void Start()
    {
        IsWhiteTurn = true;
    }

    public void SwitchTurn()
    {
        IsWhiteTurn = !IsWhiteTurn;
        Debug.Log(IsWhiteTurn ? "White's Turn" : "Black's Turn");
    }
}