using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorqueTest : MonoBehaviour
{
    public bool up = false;
    public float torque;
    public Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float turn = Input.GetAxis("Horizontal");
        if (up)
        {
            rb.AddTorque(transform.up * torque);
        }
        else
        {
            rb.AddTorque(-1 * transform.up * torque);
        }
       
    }
}
