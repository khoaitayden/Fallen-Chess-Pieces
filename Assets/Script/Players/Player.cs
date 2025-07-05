public abstract class Player
{
    public readonly bool IsWhite;
    public readonly PlayerType Type;

    protected Player(bool isWhite, PlayerType type)
    {
        IsWhite = isWhite;
        Type = type;
    }
    public abstract void OnTurnStart();
}