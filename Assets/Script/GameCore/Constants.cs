using System.Collections.Generic;
public static class Constants
{
    public const int BOARD_SIZE = 8;
    public const float PIECE_MOVE_SPEED = 15f;
    public const float DEFAULT_GAME_TIME = 600f;
}

public enum PieceType { Pawn, Rook, Knight, Bishop, Queen, King }
public enum GameStatus { Playing, Check, Checkmate, Stalemate, Draw }
public enum GameMode { Local, AI, Online }
public enum PlayerType { Human, AI, Remote }
public enum MoveType { Normal, Castle, EnPassant, Promotion }
public enum GameState { Playing, Checkmate, Stalemate, Draw, Timeout, Promotion }
public enum AIDifficulty { Easy, Normal, Hard }
public static class PieceValues
{
    public static readonly Dictionary<PieceType, int> Values = new Dictionary<PieceType, int>
    {
        { PieceType.Pawn, 10 },
        { PieceType.Knight, 30 },
        { PieceType.Bishop, 30 },
        { PieceType.Rook, 50 },
        { PieceType.Queen, 90 },
        { PieceType.King, 900 } 
    };
}