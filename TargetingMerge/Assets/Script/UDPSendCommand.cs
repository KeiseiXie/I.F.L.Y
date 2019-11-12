using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDPSendCommand : MonoBehaviour
{
    private static int localPort;

    // prefs
    public string IP = "172.20.10.3";  // define in init
    public int port = 8888;  // define in init

    // framerate
    public int framerate = 10;
    private float frameDuration;
    private float frameTimer = 0;

    // "connection" things
    IPEndPoint remoteEndPoint;
    UdpClient client;

    // gui
    string MoveMsg;
    string SpeedMsg;
    string strMessage = " ";

    public bool[] motorState;

    [Range(-255, 255)]
    public int[] motorSpeed;



    public void Start()
    {
        // framerate
        frameDuration = 1.0f / framerate;

        // ----------------------------+
        // Senden
        // ----------------------------
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
        client = new UdpClient();

        // status
        print("Sending to " + IP + " : " + port);
        print("Testing: nc -lu " + IP + " : " + port);

        motorState = new bool[4];
        motorSpeed = new int[4];
        for (int i = 0; i < 4; i++)
        {
            motorState[i] = false;
            motorSpeed[i] = 150;
        }
    }



    void Update()
    {
        //for (int i = 0; i < 4; i++)
        //{
        //    motorState[i] = false;
        //}

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //motorState[0] = true;
            motorState[0] = !motorState[0];
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            //motorState[1] = true;
            motorState[1] = !motorState[1];
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            //motorState[2] = true;
            motorState[2] = !motorState[2];
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            //motorState[3] = true;
            motorState[3] = !motorState[3];
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < 4; i++)
            {
                motorState[i] = false;
            }
        }


        // framerate
        frameTimer += Time.deltaTime;
        if (frameTimer >= frameDuration)
        {
            frameTimer = 0;
            strMessage = "";
            for (int i = 0; i < 4; i++)
            {
                if (motorState[i])
                {
                    strMessage += motorSpeed[i];
                }
                else
                {
                    strMessage += "0";
                }
                if (i < 3)
                    strMessage += "&";
            }
            Debug.Log("sending value  " + strMessage);
            sendString(IP, strMessage);
        }
    }

    private void broadcastIP()
    {

    }


    // sendData
    private void sendString(string ipAddr, string _message)
    {
        try
        {
            //if (message != "")
            //{

            // Encoding data with UTF8 encoding into binary format.
            byte[] data = Encoding.UTF8.GetBytes(_message);
            IPEndPoint dst = new IPEndPoint(IPAddress.Parse(ipAddr), port);

            // Send the text to the remote client..
            client.Send(data, data.Length, dst);
            //}
        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }
}



