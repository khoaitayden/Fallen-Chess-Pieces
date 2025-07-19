using UnityEngine;
using Mirror;
using Mirror.Discovery;
using System;
using System.Net;


public class CustomNetworkDiscovery : NetworkDiscoveryBase<DiscoveryRequest, DiscoveryResponse>
{
    public System.Collections.Generic.Dictionary<long, DiscoveryResponse> discoveredServers = new System.Collections.Generic.Dictionary<long, DiscoveryResponse>();

    protected override DiscoveryResponse ProcessRequest(DiscoveryRequest request, IPEndPoint endpoint)
    {
        CustomNetworkManager cnm = NetworkManager.singleton as CustomNetworkManager;
        
        return new DiscoveryResponse
        {
            serverId = ServerId,
            uri = transport.ServerUri(),
            roomName = string.IsNullOrEmpty(cnm.RoomName) ? "Chess Game" : cnm.RoomName,
            playerCount = NetworkManager.singleton.numPlayers,
            hasPassword = !string.IsNullOrEmpty(cnm.RoomPassword)
        };
    }

    protected override void ProcessResponse(DiscoveryResponse response, IPEndPoint endpoint)
    {
        discoveredServers[response.serverId] = response;
        OnlineUI.Instance?.UpdateServerList();
    }
}