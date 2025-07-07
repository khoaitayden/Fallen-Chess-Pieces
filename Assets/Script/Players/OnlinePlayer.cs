using UnityEngine;

public class OnlinePlayer : Player
{
    public OnlinePlayer(bool isWhite) : base(isWhite, PlayerType.Remote) { }

    public override void OnTurnStart()
    {
        Debug.Log($"Waiting for remote player ({(IsWhite ? "White" : "Black")}) to move.");
    }
}