using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class UDPDebug : MonoBehaviour
{
    private UdpClient udpClient;
    private int port = 4210;

    void Start()
    {
        udpClient = new UdpClient(port);
        _ = StartListening(); // Start the async task
    }

    private async Task StartListening()
    {
        while (true)
        {
            try
            {
                // Wait for a packet asynchronously
                UdpReceiveResult result = await udpClient.ReceiveAsync();
                string message = Encoding.ASCII.GetString(result.Buffer);

                // IMPORTANT: You cannot call most Unity API methods (like transform.position) 
                // directly from here. You must queue the data to be processed in Update().
                Debug.Log($"Received: {message} from {result.RemoteEndPoint}");
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
                break;
            }
        }
    }

    void OnDestroy()
    {
        udpClient?.Close();
    }
}
