using UnityEngine;
using Mirror;
using System.Linq;

public class CustomNetworkManager : NetworkManager
{
    public string RoomName { get; set; }
    public string RoomPassword { get; set; }


    public override void OnStartServer()
    {
        base.OnStartServer();
        NetworkServer.RegisterHandler<AuthRequestMessage>(OnAuthRequest, false);
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        if (conn == NetworkServer.localConnection)
        {
            conn.isAuthenticated = true;
            OnServerReady(conn);
        }
    }

    private void OnAuthRequest(NetworkConnectionToClient conn, AuthRequestMessage msg)
    {
        if (conn.isAuthenticated) return;

        if (string.IsNullOrEmpty(RoomPassword) || msg.password == RoomPassword)
        {
            conn.isAuthenticated = true;
        }
        else
        {
            conn.Send(new DisconnectMessage { reason = "Incorrect password." });
            conn.Disconnect();
        }
    }

    public override void OnServerReady(NetworkConnectionToClient conn)
    {
        base.OnServerReady(conn);

        if (conn.isAuthenticated)
        {
            if (conn.identity == null)
            {
                GameObject playerObj = Instantiate(playerPrefab);
                playerObj.name = $"NetworkPlayer [connId={conn.connectionId}]";
                NetworkServer.AddPlayerForConnection(conn, playerObj);
            }
        }
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        ServerUpdateLobby();
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);
        ServerUpdateLobby();
    }

    public void ServerUpdateLobby()
    {
        NetworkPlayer[] players = FindObjectsByType<NetworkPlayer>(FindObjectsSortMode.None);
        foreach (var player in players)
        {
            bool isHost = player.isServer;
            player.RpcUpdateLobbyUI(players.Length, isHost);
        }
    }

    public void ServerStartGame()
    {
        if (numPlayers == 2)
        {
            NetworkPlayer[] players = FindObjectsByType<NetworkPlayer>(FindObjectsSortMode.None);
            if (players.Length == 2)
            {
                players[0].RpcStartGame(true);
                players[1].RpcStartGame(false);
            }
        }
    }

    public void Shutdown()
    {
        GetComponent<CustomNetworkDiscovery>()?.StopDiscovery();
        if (NetworkServer.active && NetworkClient.isConnected) StopHost();
        else if (NetworkClient.isConnected) StopClient();
        else if (NetworkServer.active) StopServer();
    }


    public override void OnStartClient()
    {
        base.OnStartClient();
        NetworkClient.RegisterHandler<DisconnectMessage>(OnDisconnectMessage);
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        
        NetworkClient.Ready();
        NetworkClient.Send(new AuthRequestMessage { password = OnlineUI.Instance.GetPasswordForJoin() });
    }

    private void OnDisconnectMessage(DisconnectMessage msg)
    {
        Debug.LogError($"Kicked from server: {msg.reason}");
    }
}