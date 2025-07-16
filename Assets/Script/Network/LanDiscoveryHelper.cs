// using UnityEngine;
// using Mirror;
// using Mirror.Discovery;
// using System;
// using System.Collections.Generic;

// public class LanDiscoveryHelper : MonoBehaviour
// {
//     public Dictionary<long, ServerResponse> DiscoveredServers { get; private set; } = new Dictionary<long, ServerResponse>();
//     public event Action OnServerListUpdated;
//     private NetworkDiscovery networkDiscovery;

//     void Awake() { networkDiscovery = GetComponent<NetworkDiscovery>(); }
//     void OnEnable() { networkDiscovery.OnServerFound.AddListener(OnServerFound); }
//     void OnDisable() { networkDiscovery.OnServerFound.RemoveListener(OnServerFound); }

//     public void OnServerFound(ServerResponse info)
//     {
//         DiscoveredServers[info.serverId] = info;
//         OnServerListUpdated?.Invoke();
//     }
//     public void StartHostAndAdvertise()
//     {
//         DiscoveredServers.Clear();
//         NetworkManager.singleton.StartHost();
//         networkDiscovery.AdvertiseServer();
//     }
//     public void StartSearching()
//     {
//         DiscoveredServers.Clear();
//         OnServerListUpdated?.Invoke();
//         networkDiscovery.StartDiscovery();
//     }
//     public void Stop() { networkDiscovery.StopDiscovery(); }
// }