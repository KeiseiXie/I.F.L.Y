

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonController : MonoBehaviour
{
    // Start is called before the first frame update
    public Rigidbody balloon;

    public Transform[] targets;
    public float[] distances;
    public Vector3[] directions;

    public Vector3 CurrentPosition;
    public Vector3 CurrentDirection;
    public Vector3 OverallTarget;

    [Header("ROTATIONAL")]
    public float RotationalError;

    //use
    public float NormalizedRotationalError;
    //use
    public Vector3 towardTargetDirection;
    public Vector3 headingXZ;
    public Vector3 headingXY;
    private float dir;
    public float angleA;
    public float angleB;

    [Header("POSITIONAL")]
    private float PositionalError;

    private float NormalizedPositionalError;



    public float towardTargetDistance;
    public float MinDistThreshold = 0.5f;
    public float MaxDistThreshold = 40;

    public static float CurrentErrorDistance;
    public float PErrorFactor;

    [Header("HEIGHT")]
    public float HeightError;


    public Vector3 vector;


    [Header("Propellers")]
    public float forceRotate;
    public float forceMove;
    public float forceLift;

    void Start()
    {
        balloon = GetComponent<Rigidbody>();
        distances = new float[targets.Length];
        directions = new Vector3[targets.Length];
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        GetCurrentPosition();
        GetCurrentDirection();

        // WE MEASURE OUR BALLON RELATIONSHIP TO THE ENVIRONEMNT
        ComputeTargetDirection();
        ComputeRotationalError();
        ComputePositionalError();
    }


    void GetCurrentDirection()
    {

        CurrentDirection = balloon.transform.forward;

        CurrentDirection.Normalize();
    }


    void GetCurrentPosition()
    {


        CurrentPosition = balloon.position;
    }

    void ComputeTargetDirection()
    {
        OverallTarget = Vector3.zero;

        for (int i=0; i< targets.Length; i++)
        {
            var targetPos = targets[i].position;

            towardTargetDirection = targetPos - CurrentPosition;

            // project the direction vector to the XZ plane 
            headingXZ = towardTargetDirection;
            headingXZ.y = 0;


            // project the direction vector to the XZ plane and XY plane
            headingXY = towardTargetDirection;
            headingXY.x = 0;
            headingXY.z = 0;

            //HeightError = dir.y;

            //distances[i] = dir.magnitude;
            //directions[i] = dir.normalized;


            //OverallTarget = directions[i];
            //CurrentErrorDistance = distances[i];

            //OverallTarget.y = 0;

        }
    }
    void ComputeRotationalError()
    {
        RotationalError = Vector3.SignedAngle(CurrentDirection, towardTargetDirection, Vector3.up);
        //NormalizedRotationalError = RotationalError / 180.0f;
        float weightRotate = 0;
        angleA = Vector3.SignedAngle(headingXZ, transform.forward, Vector3.up) / 180.0f;
        angleB = Vector3.SignedAngle(headingXZ, -transform.forward, Vector3.up) / 180.0f;
        weightRotate = Mathf.Abs(angleA) < Mathf.Abs(angleB) ? angleA : angleB;

        forceRotate =Mathf.Clamp(weightRotate , -1,1);
        PErrorFactor = 1 - Mathf.Abs(forceRotate);

        Debug.DrawRay(CurrentPosition, CurrentDirection, Color.yellow);
        Debug.DrawRay(transform.position, headingXZ, Color.red);


        //Debug.Log("forceRotate" + forceRotate);

    }
    void ComputePositionalError()
    {
        //    PositionalError = CurrentErrorDistance;
        //    NormalizedPositionalError = Mathf.InverseLerp(MinDistThreshold, MaxDistThreshold, PositionalError);
        var distanceXZ = Mathf.Abs(headingXZ.magnitude);
        float weightMove = Mathf.Clamp(distanceXZ / MaxDistThreshold ,-1,1);
        weightMove *= Mathf.Sign(Vector3.Dot(headingXZ, transform.forward));


        forceMove = weightMove * PErrorFactor;
        

        //Debug.Log("forceMove" + forceMove);

        // P ERROR DISTANCE XY
        // NEEDS A SIGN
        var distanceXY = headingXY.magnitude;
        float weightLift = distanceXY / MaxDistThreshold;
        weightLift *= Mathf.Sign(Vector3.Dot(headingXY, transform.up));

       
        forceLift = weightLift ;
        //Debug.Log("weightLift" + ":" + weightLift);

    }
}
