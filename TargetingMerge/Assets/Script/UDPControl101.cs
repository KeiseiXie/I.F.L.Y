using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDPControl101 : MonoBehaviour
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

    [Range(0, 12)]
    public int MoveFactor;
    public int LiftFactor;

    public int MoveFastSpeed;
    public int MoveSlowSpeed;
    public int LiftSpeed;



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

        MoveFastSpeed = 20;
        MoveSlowSpeed = 15;
        LiftSpeed = 15;


        MoveFactor = 10;

        LiftFactor = 10;




        for (int i = 0; i < 4; i++)
        {
            motorState[i] = false;
            motorSpeed[i] = 0;
        }
    }



    void Update()
    {
        //for (int i = 0; i < 4; i++)
        //{
        //    motorState[i] = false;
        //}
        // Move Forward
        if (Input.GetKey(KeyCode.W))
        {
            //motorState[0] = true;
            motorState[0] = true;
            motorState[2] = true;

            motorSpeed[0] = MoveFastSpeed * MoveFactor;
            motorSpeed[2] = MoveFastSpeed * MoveFactor;

            // Move Forward and Turn Left
            if (Input.GetKey(KeyCode.A))
            {
                motorState[0] = true;
                motorState[2] = true;

                motorSpeed[0] = MoveFastSpeed * MoveFactor;
                motorSpeed[2] = MoveSlowSpeed * MoveFactor;
            }

            if (Input.GetKey(KeyCode.D))
            {
                motorState[0] = true;
                motorState[2] = true;

                motorSpeed[0] = MoveSlowSpeed * MoveFactor;
                motorSpeed[2] = MoveFastSpeed * MoveFactor;
            }
        }


        if (Input.GetKey(KeyCode.S))
        {
            //motorState[0] = true;
            motorState[0] = true;
            motorState[2] = true;

            motorSpeed[0] = -1 * MoveFastSpeed * MoveFactor;
            motorSpeed[2] = -1 * MoveFastSpeed * MoveFactor;

            // Move Backrward and Turn Left
            if (Input.GetKey(KeyCode.A))
            {
                motorState[0] = true;
                motorState[2] = true;

                motorSpeed[0] = -1 * MoveFastSpeed * MoveFactor;
                motorSpeed[2] = -1 * MoveSlowSpeed * MoveFactor;
            }

            if (Input.GetKey(KeyCode.D))
            {
                motorState[0] = true;
                motorState[2] = true;

                motorSpeed[0] = -1 * MoveSlowSpeed * MoveFactor;
                motorSpeed[2] = -1 * MoveFastSpeed * MoveFactor;
            }
        }


        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D))
        {

            motorState[0] = false;
            motorState[2] = false;

            motorSpeed[0] = 0;
            motorSpeed[2] = 0;
        }



        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int i = 0; i < 4; i++)
            {
                motorState[i] = false;
                motorSpeed[i] = 0;
            }
        }


        //Lift and Down

        if (Input.GetKeyDown(KeyCode.Q))
        {

            motorState[1] = !motorState[1];
            motorState[3] = !motorState[3];
            motorSpeed[1] = LiftSpeed * LiftFactor;
            motorSpeed[3] = LiftSpeed * LiftFactor;

        }

        if (Input.GetKeyDown(KeyCode.E))
        {

            motorState[1] = !motorState[1];
            motorState[3] = !motorState[3];
            motorSpeed[1] = -1 * LiftSpeed * LiftFactor;
            motorSpeed[3] = -1 * LiftSpeed * LiftFactor;

        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            MoveFactor = MoveFactor + 1;
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            MoveFactor = MoveFactor - 1;
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            LiftFactor = LiftFactor + 1;
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            LiftFactor = LiftFactor - 1;
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



