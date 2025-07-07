// In NetworkPlayer.cs
using UnityEngine;
using Mirror;

public class NetworkPlayer : NetworkBehaviour
{
    [ClientRpc]
    public void RpcStartGame(bool iAmWhite)
    {
        if (!isOwned) return;

        Debug.Log($"Received StartGame command. I am {(iAmWhite ? "White" : "Black")}");
        
        // Pass our NetworkMoveRelay component to the GameManager so the HumanPlayer can use it.
        NetworkMoveRelay myRelay = GetComponent<NetworkMoveRelay>();
        GameManager.Instance.StartOnlineGame(iAmWhite, myRelay);
    }
}