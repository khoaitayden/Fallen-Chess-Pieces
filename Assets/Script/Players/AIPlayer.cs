using System.Collections;
using System.Collections.Generic;
using System.Linq; // Required for .ToList() and .Count()
using UnityEngine;

public class AIPlayer : Player
{
    private Chessboard _chessboard;

    public AIPlayer(bool isWhite) : base(isWhite, PlayerType.AI) 
    {
        _chessboard = Object.FindObjectOfType<Chessboard>();
    }

    public override void OnTurnStart()
    {
        Chessboard.Instance.StartCoroutine(DelayedMove());
    }

    private IEnumerator DelayedMove()
    {
        yield return new WaitForSeconds(0.5f); // A short delay

        // --- AI LOGIC: LEVEL 1 (RANDOM MOVER) ---

        // 1. Find all of my pieces that can actually move.
        List<ChessPiece> myMovablePieces = new List<ChessPiece>();
        for (int x = 0; x < Constants.BOARD_SIZE; x++)
        {
            for (int y = 0; y < Constants.BOARD_SIZE; y++)
            {
                ChessPiece piece = _chessboard.GetPieceAt(new Vector2Int(x, y));
                if (piece != null && piece.IsWhite == this.IsWhite)
                {
                    if (MoveValidator.Instance.GetValidMoves(piece).Count > 0)
                    {
                        myMovablePieces.Add(piece);
                    }
                }
            }
        }

        // If there are no movable pieces, the game should have already ended (stalemate/checkmate).
        if (myMovablePieces.Count == 0)
        {
            Debug.LogWarning("AI has no valid moves, but the game hasn't ended.");
            yield break;
        }

        // 2. Pick a random piece from the list of movable pieces.
        ChessPiece randomPiece = myMovablePieces[Random.Range(0, myMovablePieces.Count)];
        
        // 3. Get its valid moves and pick a random one.
        List<Vector2Int> validMoves = MoveValidator.Instance.GetValidMoves(randomPiece);
        Vector2Int randomMove = validMoves[Random.Range(0, validMoves.Count)];

        // 4. Execute the move (this is the same logic as HumanPlayer's HandleAction).
        Debug.Log($"AI moves {randomPiece.Type} to {randomMove}");
        
        Vector2Int oldPosition = randomPiece._boardPosition;
        _chessboard.MovePiece(randomPiece, randomMove);
        TurnManager.Instance.SetEnPassantTarget(randomPiece, oldPosition, randomMove);
        TurnManager.Instance.SwitchTurn();

        // Record history, check for game end, and notify the next player.
        string notation = MoveConverter.ToDescriptiveNotation(randomPiece, randomMove);
        MoveData move = new MoveData(randomPiece.Type, oldPosition, randomMove, notation);
        MoveHistory.Instance.AddMove(move);
        GameManager.Instance.CheckForGameEnd();

        if (GameManager.Instance.CurrentState == GameState.Playing)
        {
            GameManager.Instance.NotifyCurrentPlayer();
        }
    }
}