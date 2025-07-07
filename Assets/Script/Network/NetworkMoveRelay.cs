// In NetworkMoveRelay.cs
using UnityEngine;
using Mirror;

public class NetworkMoveRelay : NetworkBehaviour
{
    [Command]
    public void CmdSendMove(Vector2Int from, Vector2Int to)
    {
        RpcReceiveMove(from, to);
    }

    [ClientRpc]
    private void RpcReceiveMove(Vector2Int from, Vector2Int to)
    {
        // Find the piece on our local board and move it.
        ChessPiece piece = Chessboard.Instance.GetPieceAt(from);
        if (piece == null)
        {
            Debug.LogError($"Move failed: No piece found at {from} on local board.");
            return;
        }

        // Since every client runs this same code, their boards will stay in sync.
        Chessboard.Instance.MovePiece(piece, to);
        TurnManager.Instance.SetEnPassantTarget(piece, from, to);
        TurnManager.Instance.SwitchTurn();

        string notation = MoveConverter.ToDescriptiveNotation(piece, to);
        MoveData move = new MoveData(piece.Type, from, to, notation);
        MoveHistory.Instance.AddMove(move);

        GameManager.Instance.CheckForGameEnd();

        if (GameManager.Instance.CurrentState == GameState.Playing)
        {
            GameManager.Instance.NotifyCurrentPlayer();
        }
    }
}