using UnityEngine;
using Mirror;
using System.Linq;
using System.Collections;

public class CustomNetworkManager : NetworkManager
{
    public string RoomName { get; set; }
    public string RoomPassword { get; set; }

    public override void Awake()
    {
        base.Awake();
        if (singleton != null && singleton != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        ServerUpdateLobby();
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);

        StartCoroutine(DelayedLobbyUpdate());
    }


    private IEnumerator DelayedLobbyUpdate()
    {
        yield return new WaitForEndOfFrame();


        ServerUpdateLobby();
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        UIManager.Instance.ShowMenuPanel();
    }

    public void ServerUpdateLobby()
    {
        NetworkPlayer[] players = FindObjectsByType<NetworkPlayer>(FindObjectsSortMode.None);
        foreach (var player in players)
        {
            bool isHost = player.connectionToClient == NetworkServer.localConnection;
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
        Debug.Log("Shutdown requested. Stopping all network activity.");

        if (TryGetComponent(out CustomNetworkDiscovery discovery))
        {
            discovery.StopDiscovery();
        }

        if (NetworkServer.active && NetworkClient.isConnected)
        {
            StopHost();
        }
        else if (NetworkClient.isConnected)
        {
            StopClient();
        }
        else if (NetworkServer.active)
        {
            StopServer();
        }
    }
}