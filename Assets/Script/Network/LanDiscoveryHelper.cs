// In LanDiscoveryHelper.cs
using UnityEngine;
using Mirror;
using Mirror.Discovery;
using System.Collections.Generic;

// We don't need custom messages for this simpler, more robust approach.

public class LanDiscoveryHelper : MonoBehaviour
{
    // We will use Mirror's built-in ServerResponse struct.
    public Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>();
    
    private NetworkDiscovery networkDiscovery;

    void Awake()
    {
        networkDiscovery = GetComponent<NetworkDiscovery>();
    }

    void OnEnable()
    {
        // Subscribe our handler method to the discovery component's event.
        networkDiscovery.OnServerFound.AddListener(OnServerFound);
    }

    void OnDisable()
    {
        networkDiscovery.OnServerFound.RemoveListener(OnServerFound);
    }

    // This method is called by the NetworkDiscovery component's event.
    public void OnServerFound(ServerResponse info)
    {
        discoveredServers[info.serverId] = info;
        OnlineUI.Instance?.UpdateServerList();
    }

    // --- Public methods for the UI to call ---

    public void StartHostAndAdvertise()
    {
        discoveredServers.Clear();
        NetworkManager.singleton.StartHost();
        networkDiscovery.AdvertiseServer();
        Debug.Log("Helper: Started hosting and advertising server.");
    }

    public void StartSearching()
    {
        discoveredServers.Clear();
        OnlineUI.Instance?.UpdateServerList(); // Clear the UI immediately
        networkDiscovery.StartDiscovery();
        Debug.Log("Helper: Started searching for servers.");
    }

    public void Stop()
    {
        networkDiscovery.StopDiscovery();
        Debug.Log("Helper: Stopped discovery/advertising.");
    }
}