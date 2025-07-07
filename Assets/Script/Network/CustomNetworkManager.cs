// In CustomNetworkManager.cs
using UnityEngine;
using Mirror;
using System.Linq;
using System.Collections;
public class CustomNetworkManager : NetworkManager
{
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        // When a player connects, check if we have reached our target number.
        if (numPlayers == 2)
        {
            // Don't check immediately. Start a coroutine that will wait a frame.
            StartCoroutine(CheckPlayersAndStartGame());
        }
    }
    
    private IEnumerator CheckPlayersAndStartGame()
    {
        yield return new WaitForEndOfFrame();

        NetworkPlayer[] players = FindObjectsOfType<NetworkPlayer>();

        if (players.Length == 2)
        {
            Debug.Log("Found two NetworkPlayer objects. Assigning colors and starting the game!");

            NetworkPlayer whitePlayer = players.OrderBy(p => p.netId).First();
            NetworkPlayer blackPlayer = players.OrderBy(p => p.netId).Last();

            whitePlayer.RpcStartGame(true); 
            blackPlayer.RpcStartGame(false); 
        }
    }

}
