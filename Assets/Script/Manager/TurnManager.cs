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
        bool previousPlayerIsWhite = IsWhiteTurn;
        IsWhiteTurn = !IsWhiteTurn;
        
        Debug.Log(IsWhiteTurn ? "White's Turn" : "Black's Turn");

        if (MoveValidator.Instance.IsCheckmate(IsWhiteTurn))
        {
            GameManager.Instance.EndGame(GameState.Checkmate, previousPlayerIsWhite);
        }
        else if (MoveValidator.Instance.IsStalemate(IsWhiteTurn))
        {
            GameManager.Instance.EndGame(GameState.Stalemate, false);
        }
    }
}