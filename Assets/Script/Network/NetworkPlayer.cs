using UnityEngine;
using Mirror;

public class NetworkPlayer : NetworkBehaviour
{
    [ClientRpc]
    public void RpcStartGame(bool iAmWhite)
    {
        if (!isOwned) return;
        NetworkMoveRelay myRelay = GetComponent<NetworkMoveRelay>();
        GameManager.Instance.StartOnlineGame(iAmWhite, myRelay);
    }
    [ClientRpc]
    public void RpcUpdateLobbyUI(int playerCount, bool isHost)
    {
        if (!isOwned) return;
        
        LobbyUI lobby = FindObjectOfType<LobbyUI>();
        if (lobby != null)
        {
            lobby.UpdateLobbyUI(playerCount, isHost);
        }
    }
}