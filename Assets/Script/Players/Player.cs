// Create new script: Player.cs
public abstract class Player
{
    public readonly bool IsWhite;
    public readonly PlayerType Type;

    protected Player(bool isWhite, PlayerType type)
    {
        IsWhite = isWhite;
        Type = type;
    }

    // This method is called by the GameManager when it's this player's turn to move.
    public abstract void OnTurnStart();
}