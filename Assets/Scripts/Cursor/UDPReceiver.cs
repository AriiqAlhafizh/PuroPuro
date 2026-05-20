using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Globalization;

public class UDPReceiver : MonoBehaviour
{
    UdpClient client;
    Thread receiveThread;

    public int port = 4210;

    public float yaw;
    public float pitch;
    public float roll;

    public bool triggerPressed;

    void Start()
    {
        receiveThread =
            new Thread(new ThreadStart(ReceiveData));

        receiveThread.IsBackground = true;

        receiveThread.Start();

        Debug.Log("UDP Receiver Started");
    }

    void ReceiveData()
    {
        client = new UdpClient(AddressFamily.InterNetwork);

        client.Client.Bind(
            new IPEndPoint(IPAddress.Any, port)
        );

        while (true)
        {
            try
            {
                IPEndPoint anyIP =
                    new IPEndPoint(IPAddress.Any, 0);

                byte[] data =
                    client.Receive(ref anyIP);

                string text =
                    Encoding.UTF8.GetString(data);

                //Debug.Log("Received: " + text);

                string[] values =
                    text.Split(',');

                if (values.Length == 4)
                {
                    yaw =
                        float.Parse(values[0], CultureInfo.InvariantCulture);

                    roll =
                        float.Parse(values[1], CultureInfo.InvariantCulture);

                    pitch =
                        float.Parse(values[2], CultureInfo.InvariantCulture);

                    triggerPressed =
                        int.Parse(values[3]) == 1;
                }
            }
            catch
            {
            }
        }
    }

    void Update()
    {
        //transform.rotation =
        //    Quaternion.Euler(
        //        pitch,
        //        yaw,
        //        -roll
        //    );
    }

    void OnApplicationQuit()
    {
        if (receiveThread != null)
            receiveThread.Abort();

        if (client != null)
            client.Close();
    }
}