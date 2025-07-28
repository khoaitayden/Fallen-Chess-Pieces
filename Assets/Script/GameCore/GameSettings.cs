public class GameSettings
{

    public float GameTime { get; set; } = 600f;

    public float TimeIncrement { get; set; } = 0f;

    public int CapturesForExtraLife { get; set; } = 8;

    public int ResurrectionTurns { get; set; } = 2;

    public int PowerLossDuration { get; set; } = 2;

    public void ResetToDefaults()
    {
        GameTime = 600f;
        TimeIncrement = 0f;
        CapturesForExtraLife = 8;
        ResurrectionTurns = 2;
        PowerLossDuration = 2;
    }
}