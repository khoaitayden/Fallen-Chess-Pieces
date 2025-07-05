// Create new script: IAIStrategy.cs
using System.Collections.Generic;

// An interface is a contract. Any class that implements IAIStrategy
// MUST provide a GetBestMove method.
public interface IAIStrategy
{
    MoveData GetBestMove(bool isWhite, Chessboard board);
}