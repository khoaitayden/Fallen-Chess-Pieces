using UnityEngine;
using Mirror;

public class NetworkPlayer : NetworkBehaviour
{
    [ClientRpc]
    public void RpcUpdateLobbyUI(int playerCount, bool amIHost)
    {
        if (!isLocalPlayer) return;
        LobbyUI lobby = FindObjectOfType<LobbyUI>();
        if (lobby != null)
        {
            lobby.UpdateLobbyUI(playerCount, amIHost);
        }
    }

    [ClientRpc]
    public void RpcStartGame(bool iAmWhite)
    {
        if (!isLocalPlayer) return;
        NetworkMoveRelay myRelay = GetComponent<NetworkMoveRelay>();
        GameManager.Instance.StartOnlineGame(iAmWhite, myRelay);
    }

    [Command]
    public void CmdLeaveLobby()
    {
        CustomNetworkManager manager = NetworkManager.singleton as CustomNetworkManager;
        if (manager == null) return;

        if (this.connectionToClient == NetworkServer.localConnection)
        {
            manager.Shutdown();
        }
        else
        {
            this.connectionToClient.Disconnect();
        }
    }
}