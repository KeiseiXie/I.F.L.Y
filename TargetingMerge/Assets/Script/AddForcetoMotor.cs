using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddForcetoMotor : MonoBehaviour
{


    public bool IsRotating;
    public const float MIN_ROT_THRES = 0.1f;

    public bool IsMoving;
    public const float MIN_MOVE_THRES = 0.1f;

    public bool IsLifting;
    public const float MIN_LIFT_THRES = 0.01f;

    public bool AddForce;

    public Rigidbody balloon;

    public GameObject Sphere;


    //The propellers
    public float propellerA;
    public float propellerB;
    public float propellerC;
    public float propellerD;




    [Header("PID Output")]
    public float PID_Height_output;

    public float PID_Rotate_output;

    public float PID_move_output;

    public float rotateError;

    public float ForwardorBackward;


    [Header("Internal")]
    public float maxPropellerForce; //100
    public float maxTorque; //1
    public float throttle;
    public float moveFactor; //5

    //PID
    public Vector3 PID_height_gains; //(2, 3, 2)
    public Vector3 PID_rotate_gains; //(2, 0.2, 0.5)
    public Vector3 PID_move_gains; //(1, 0, 0)

    //The PID controllers
    private PIDController PID_height;
    private PIDController PID_rotate;
    private PIDController PID_move;



    [Header("Motors")]
    public bool[] motorState;
    public float[] motorSpeed;
    

    //Movement factors
    public float moveForwardBack;
    public float moveHeight;

    public float MaxC;
    public float MaxA;

    public bool useHeightPID = true;
    public bool useRotatePID = true;
    public bool useMovePID = true;
    // Start is called before the first frame update
    void Start()
    {
        balloon = transform.GetComponent<Rigidbody>();
        PID_height = new PIDController();
        PID_rotate = new PIDController();
        PID_move = new PIDController();

        motorState = new bool[4];
        motorSpeed = new float[4];

        for (int i = 0; i < 4; i++)
        {
            motorState[i] = false;
            motorSpeed[i] = 0;
        }

        //PID_height_gains = new Vector3(1, 1, 1);
        //PID_rotate_gains = new Vector3(1, 1, 1);
        //PID_move_gains = new Vector3(1, 1, 1);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            
        }
        
        CalculateForces();
        CheckState();
        ControlPropellers(PropellerType.A_C);
        ControlPropellers(PropellerType.B_D);

        ComputeMotorForce();
    }


    void CalculateForces()
    {
        rotateError = Sphere.GetComponent<BalloonController>().forceRotate;

        float heightError = Sphere.GetComponent<BalloonController>().forceLift;


        float positionError = Sphere.GetComponent<BalloonController>().forceMove;






        // B AND D
        if (useHeightPID)
        {
            PID_Height_output = PID_height.GetFactorFromPIDController(PID_height_gains,heightError);
        }
        else
        {
            PID_Height_output = PID_height.GetFactorFromPIDController(new Vector3(0, 0, 0), heightError);
        }
        
        propellerD = PID_Height_output;
        propellerB = PID_Height_output;

        //Debug.Log("PID_Height_output : " + PID_Height_output);

        //A AND C
        if (useRotatePID)
        {
            PID_Rotate_output = PID_move_output = PID_rotate.GetFactorFromPIDController(PID_rotate_gains, rotateError);   
        }
        else
        {
            PID_Rotate_output = PID_move_output = PID_rotate.GetFactorFromPIDController(new Vector3(0, 0, 0), rotateError);
        }

        if (useMovePID)
        {
            PID_move_output = PID_move.GetFactorFromPIDController(PID_move_gains, positionError);
        }
        else
        {
            PID_move_output = PID_move.GetFactorFromPIDController(new Vector3(0, 0, 0), positionError);
        }

        if(PID_Rotate_output >= 0)
        {
            propellerC = PID_move_output - PID_Rotate_output;
            propellerA = PID_move_output + PID_Rotate_output;
        }

        else if (-90 < PID_Rotate_output && PID_Rotate_output < 0)
        {
            propellerC = PID_move_output - PID_Rotate_output;
            propellerA = PID_move_output + PID_Rotate_output;
        }
        else
        {
            propellerC = PID_move_output + PID_Rotate_output;
            propellerA = PID_move_output - PID_Rotate_output;
        }

        if(propellerC > MaxC)
        {
            MaxC = propellerC;
        }
        //Debug.Log(" MAXC :" + MaxC);

        if (propellerA > MaxA)
        {
            MaxA = propellerA;
        }
        //Debug.Log(" MAXA :" + MaxA);


    }

    void CheckState()
    {
        IsRotating = true;
        IsMoving = false;
        IsLifting = true;

        if (Mathf.Abs(PID_Rotate_output) < MIN_ROT_THRES)
        {
            IsRotating = false;

        }
        else
        {
            IsRotating = true;

        }
        if (Mathf.Abs(PID_move_output) < MIN_MOVE_THRES)
        {
            IsMoving = false;

        }
        else
        {
            IsMoving = true;

        }
        //if (Mathf.Abs(PID_move_output) < MIN_MOVE_THRES)
        //{
        //    IsMoving = false;
        //}


        if (Mathf.Abs(PID_Height_output) < MIN_LIFT_THRES)
        {
            IsLifting = false;
        }

    }

    void ControlPropellers(PropellerType type)
    {
        if (!AddForce) return;

        switch (type)
        {
            case PropellerType.A_C:
                if (IsRotating)
                {
                    
                    //Debug.Log("transform.up, PID_Rotate_output ： " + transform.up + ", " + PID_Rotate_output);
                    
                    ApplyTorque(-transform.up, PID_Rotate_output);
                    
                    //ApplyTorque(transform.up, propellerA);

                    // Apply  to Motors


                }

                if (IsMoving)
                {



                    ApplyForce(transform.forward, PID_move_output);



                    // Apply  to Motors
                    //Debug.Log("should move");

                }
                break;

            case PropellerType.B_D:
                if (IsLifting)
                {

                        ApplyForce(transform.up, propellerB * 10);
                        // Apply  to Motors

                    
                }
                break;
        }
    }


    void ApplyTorque(Vector3 axis, float magnitude)
    {
        balloon.AddTorque(axis * magnitude, ForceMode.Impulse);
        //for rotate
    }

    void ApplyForce(Vector3 direction, float magnitude)
    {
        balloon.AddForce(direction * magnitude);
        //for lift
    }


    void ComputeMotorForce()
    {
        if (Mathf.Abs(propellerA)> 0 && Mathf.Abs(propellerC) >0)
        {
            motorState[0] = true;
            motorState[2] = true;

            int a = Mathf.RoundToInt(propellerA * moveForwardBack);
            int c = Mathf.RoundToInt(propellerC * moveForwardBack);

            a = Mathf.Clamp(a, -255, 255);
            c = Mathf.Clamp(c, -255, 255);
            float BiggerValue = Mathf.Abs(c) > Mathf.Abs(a) ? c : a;

            if(BiggerValue > 0)
            {
                a = Mathf.Abs(a);
                c = Mathf.Abs(c);
            }
            else
            {
                a = -Mathf.Abs(a);
                c = -Mathf.Abs(c);
            }

            motorSpeed[0] = a;
            motorSpeed[2] = c;

        }
        else
        {
            motorState[0] = false;
            motorState[2] = false;
            motorSpeed[0] = 0;
            motorSpeed[2] = 0;
        }

        if (Mathf.Abs(propellerB)> 0)
        {
            motorState[1] = true;
            motorState[3] = true;

            float b = Mathf.RoundToInt(propellerB * moveHeight);
            float d =Mathf.RoundToInt(propellerD * moveHeight);

            b = Mathf.Clamp(b, -255, 255);
            d = Mathf.Clamp(d, -255, 255);

            //Debug.Log("float b: " + b);
            //Debug.Log("float d: " + d); 



            motorSpeed[1] = b;
            motorSpeed[3] = d;
        }
        else
        {
            motorState[1] = false;
            motorState[3] = false;
            motorSpeed[1] = 0;
            motorSpeed[3] = 0;
        }

    }


}
public enum PropellerType
{
    A_C,
    B_D //Lift
}
