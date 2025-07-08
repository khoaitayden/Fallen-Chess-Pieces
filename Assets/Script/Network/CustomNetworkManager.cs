using UnityEngine;
using Mirror;
using System.Linq;

public class CustomNetworkManager : NetworkManager
{
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        ServerUpdateLobby();
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);
        ServerUpdateLobby();
    }

    public void ServerStartGame()
    {
        if (numPlayers == 2)
        {
            NetworkPlayer[] players = FindObjectsOfType<NetworkPlayer>();
            players[0].RpcStartGame(true);
            players[1].RpcStartGame(false);
        }
    }
    public void ServerUpdateLobby()
    {
        NetworkPlayer[] players = FindObjectsOfType<NetworkPlayer>();
        foreach (var player in players)
        {
            player.RpcUpdateLobbyUI(numPlayers, player.isOwned && player.isServer);
        }
    }
}