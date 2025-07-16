using System.Net;
using System.Net.Sockets;
using System.Linq;
public static class NetworkUtils
{
    private static readonly System.Random _random = new System.Random();
    private const string ALPHANUMERIC = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public static string GenerateRoomCode(int length)
    {
        return new string(Enumerable.Repeat(ALPHANUMERIC, length)
            .Select(s => s[_random.Next(s.Length)]).ToArray());
    }
    public static string GetLocalIPv4()
    {
        string localHostName = Dns.GetHostName();
        try
        {
            IPHostEntry hostEntry = Dns.GetHostEntry(localHostName);
            foreach (IPAddress ip in hostEntry.AddressList)
            {
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
        return "localhost";
    }
}