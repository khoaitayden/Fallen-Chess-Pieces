// Create new script: NetworkUtils.cs
using System.Net;
using System.Net.Sockets;

public static class NetworkUtils
{
    // This method finds the local IPv4 address of the machine.
    public static string GetLocalIPv4()
    {
        // Get the local host name
        string localHostName = Dns.GetHostName();
        try
        {
            // Get the IP addresses associated with the host name
            IPHostEntry hostEntry = Dns.GetHostEntry(localHostName);
            foreach (IPAddress ip in hostEntry.AddressList)
            {
                // We are looking for an IPv4 address
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
        }
        catch (SocketException ex)
        {
            UnityEngine.Debug.LogError($"Could not get local IP address: {ex.Message}");
        }
        // Fallback to localhost if no suitable IP is found
        return "localhost";
    }
}