using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class isGrounded : MonoBehaviour
{

    public PlayerMovement playerMovement;

    public bool isGroundedBool = false;

    public bool isRamp = false;

    public Rigidbody rb;

    public CarScript carScript;


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Ground"))
        {
            isGroundedBool = true;
            isRamp = false;
        }
        else if(other.tag.Equals("Ramp"))
        {
            isRamp = true;
            isGroundedBool = false;
        }
    }
}
