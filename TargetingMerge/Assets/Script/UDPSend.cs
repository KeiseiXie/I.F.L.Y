using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDPSend : MonoBehaviour
{
    private static int localPort;
    public Boolean print;

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

    //refer to PID
    public GameObject balloon;
    public bool[] State;
    public float[] Speed;

    public bool Stop;



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

        State = new bool[4];
        Speed = new float[4];
        for (int i = 0; i < 4; i++)
        {
            State[i] = false;
            Speed[i] = 0;
        }
    }



    void Update()
    {
        //get the motorstate from the PID scripts
        if (Stop)
        {
            for (int i = 0; i < 4; i++)
            {
                State[i] = false;
                Speed[i] = 0;
            }
          
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                //State[i] = balloon.GetComponent<BalloonController>().motorState[i];
                //Speed[i] = balloon.GetComponent<BalloonController>().motorSpeed[i];

                State[i] = balloon.GetComponent<AddForcetoMotor>().motorState[i];
                Speed[i] = balloon.GetComponent<AddForcetoMotor>().motorSpeed[i];

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
                if (State[i])
                {
                    strMessage += Speed[i];
                }
                else
                {
                    strMessage += "0";
                }
                if (i < 3)
                    strMessage += "&";
            }
            if (print)
            {
                Debug.Log("sending value  " + strMessage);
            }
            
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


