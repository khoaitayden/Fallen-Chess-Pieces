using Mirror;
using System;

public struct AuthRequestMessage : NetworkMessage
{
    public string password;
}

public struct AuthResponseMessage : NetworkMessage
{
    public bool success;
    public string reason;
}

public struct DisconnectMessage : NetworkMessage
{
    public string reason;
}

public struct DiscoveryRequest : NetworkMessage { }

public struct DiscoveryResponse : NetworkMessage
{
    public long serverId;
    public Uri uri;
    public string roomName;
    public int playerCount;
    public bool hasPassword;
}