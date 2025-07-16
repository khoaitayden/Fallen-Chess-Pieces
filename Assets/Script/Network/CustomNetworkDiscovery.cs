using UnityEngine;
using Mirror;
using Mirror.Discovery;
using System;
using System.Net;

public class CustomNetworkDiscovery : NetworkDiscoveryBase<DiscoveryRequest, DiscoveryResponse>
{
    public System.Collections.Generic.Dictionary<long, DiscoveryResponse> discoveredServers = new System.Collections.Generic.Dictionary<long, DiscoveryResponse>();

    #region Server
    protected override DiscoveryResponse ProcessRequest(DiscoveryRequest request, IPEndPoint endpoint)
    {
        try
        {
            CustomNetworkManager cnm = NetworkManager.singleton as CustomNetworkManager;
            return new DiscoveryResponse
            {
                serverId = ServerId,
                uri = new Uri($"kcp://{NetworkUtils.GetLocalIPv4()}"),
                roomName = string.IsNullOrEmpty(cnm.RoomName) ? "Chess Game" : cnm.RoomName,
                playerCount = NetworkManager.singleton.numPlayers,
                hasPassword = !string.IsNullOrEmpty(cnm.RoomPassword)
            };
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to process discovery request: {e.Message}");
            return new DiscoveryResponse();
        }
    }
    #endregion

    #region Client
    protected override void ProcessResponse(DiscoveryResponse response, IPEndPoint endpoint)
    {
        discoveredServers[response.serverId] = response;
        OnlineUI.Instance?.UpdateServerList();
    }
    #endregion
}