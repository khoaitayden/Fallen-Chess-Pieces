using System.Collections.Generic;

public interface IAIStrategy
{
    MoveData GetBestMove(bool isWhite, Chessboard board);
}