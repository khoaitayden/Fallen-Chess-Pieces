public static class Constants
{
    public const int BOARD_SIZE = 8;
    public const float PIECE_MOVE_SPEED = 10f;
    public const float DEFAULT_GAME_TIME = 600f;
}

public enum PieceType { Pawn, Rook, Knight, Bishop, Queen, King }
public enum GameStatus { Playing, Check, Checkmate, Stalemate, Draw }
public enum GameMode { Local, Online, AI }
public enum MoveType { Normal, Castle, EnPassant, Promotion }