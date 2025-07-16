// // In CustomAuthenticator.cs
// using UnityEngine;
// using Mirror;

// public class CustomAuthenticator : NetworkAuthenticator
// {
//     [Header("Authentication")]
//     public string roomPassword;

//     #region Server
//     public override void OnStartServer()
//     {
//         // Register the handler for the authentication request message from remote clients.
//         // The 'false' at the end allows this message from unauthenticated clients.
//         NetworkServer.RegisterHandler<AuthRequestMessage>(OnAuthRequest, false);
//     }

//     public override void OnStopServer()
//     {
//         NetworkServer.UnregisterHandler<AuthRequestMessage>();
//     }

//     // This is called on the server when a new client connects.
//     public override void OnServerAuthenticate(NetworkConnectionToClient conn)
//     {
//         // The host is always trusted. We accept them and immediately
//         // tell their client side that authentication was successful.
//         if (conn == NetworkServer.localConnection)
//         {
//             conn.Send(new AuthResponseMessage { success = true, reason = "Host authenticated locally." });
//             ServerAccept(conn);
//         }
//         // For remote clients, we do nothing and wait for their AuthRequestMessage.
//     }

//     public void OnAuthRequest(NetworkConnectionToClient conn, AuthRequestMessage msg)
//     {
//         if (string.IsNullOrEmpty(roomPassword) || msg.password == roomPassword)
//         {
//             // Tell the client their authentication was successful.
//             conn.Send(new AuthResponseMessage { success = true, reason = "Authentication successful." });
//             ServerAccept(conn);
//         }
//         else
//         {
//             conn.Send(new AuthResponseMessage { success = false, reason = "Incorrect password." });
//             ServerReject(conn);
//         }
//     }
//     #endregion

//     #region Client
//     public override void OnStartClient()
//     {
//         NetworkClient.RegisterHandler<AuthResponseMessage>(OnAuthResponse, false);
//         NetworkClient.RegisterHandler<DisconnectMessage>(OnDisconnectMessage, false);
//     }

//     public override void OnStopClient()
//     {
//         NetworkClient.UnregisterHandler<AuthResponseMessage>();
//         NetworkClient.UnregisterHandler<DisconnectMessage>();
//     }

//     public override void OnClientAuthenticate()
//     {
//         // We now send the auth request for BOTH host and client.
//         // The server will handle the host's case specially.
//         string pass = OnlineUI.Instance.GetPasswordForJoin();
//         NetworkClient.Send(new AuthRequestMessage { password = pass });
//     }

//     // This is the new, crucial part of the handshake on the client.
//     public void OnAuthResponse(AuthResponseMessage msg)
//     {
//         if (msg.success)
//         {
//             Debug.Log("<color=green>Authentication Success! Sending Ready message to server.</color>");
//             // If the server confirmed our authentication, NOW we are ready to enter the game.
//             // This calls base.OnClientAuthenticated and sets NetworkClient.ready = true.
//             ClientAccept();
//         }
//         else
//         {
//             Debug.LogError($"Authentication Failed: {msg.reason}");
//             // The server will disconnect us, but we can stop the client early.
//             NetworkManager.singleton.StopClient();
//         }
//     }

//     private void OnDisconnectMessage(DisconnectMessage msg)
//     {
//         Debug.LogError($"Kicked from server: {msg.reason}");
//         NetworkManager.singleton.StopClient();
//     }
//     #endregion
// }