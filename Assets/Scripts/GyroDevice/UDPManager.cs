using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Globalization;

public class UDPManager : MonoBehaviour
{
    UdpClient udp;
    Thread receiveThread;

    [Header("Ports")]
    public int localPort = 4210;
    public int esp32Port = 4211;

    [Header("ESP32")]
    public string esp32IP = "192.168.1.50";

    // RECEIVED DATA
    public float yaw;
    public float pitch;

    public bool triggerPressed;

    string latestPacket = "";

    void Start()
    {
        // CREATE SINGLE UDP CLIENT
        udp =
            new UdpClient(AddressFamily.InterNetwork);

        udp.Client.Bind(
            new IPEndPoint(
                IPAddress.Any,
                localPort
            )
        );

        // START RECEIVE THREAD
        receiveThread =
            new Thread(ReceiveData);

        receiveThread.IsBackground = true;

        receiveThread.Start();

        Debug.Log("UDP Manager Started");
    }

    // =========================
    // RECEIVE DATA
    // =========================
    void ReceiveData()
    {
        while (true)
        {
            try
            {
                IPEndPoint anyIP =
                    new IPEndPoint(
                        IPAddress.Any,
                        0
                    );

                byte[] data =
                    udp.Receive(ref anyIP);

                latestPacket =
                    Encoding.UTF8.GetString(data);
            }
            catch (System.Exception e)
            {
                Debug.Log(e.ToString());
            }
        }
    }

    // =========================
    // MAIN THREAD
    // =========================
    void Update()
    {
        if (!string.IsNullOrEmpty(latestPacket))
        {
            // DEBUG
            //Debug.Log(latestPacket);

            string[] values =
                latestPacket.Split(',');

            // EXPECT:
            // yaw,pitch,trigger
            if (values.Length == 3)
            {
                yaw =
                    float.Parse(
                        values[0],
                        CultureInfo.InvariantCulture
                    );

                pitch =
                    float.Parse(
                        values[1],
                        CultureInfo.InvariantCulture
                    );

                triggerPressed =
                    int.Parse(values[2]) == 1;
            }

            latestPacket = "";
        }
    }

    // =========================
    // SEND AMMO TO ESP32
    // =========================
    public void SendAmmo(int ammo)
    {
        try
        {
            string message =
                ammo.ToString();

            byte[] data =
                Encoding.UTF8.GetBytes(message);

            udp.Send(
                data,
                data.Length,
                esp32IP,
                esp32Port
            );

            Debug.Log(
                "Sent Ammo: " + ammo
            );
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    // =========================
    // CLEANUP
    // =========================
    void OnApplicationQuit()
    {
        if (receiveThread != null)
            receiveThread.Abort();

        if (udp != null)
            udp.Close();
    }
}