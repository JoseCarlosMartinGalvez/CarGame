using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class isGrounded : MonoBehaviour
{

    public PlayerMovement playerMovement;

    public Rigidbody rb;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Ground"))
        {
            print("isground");
        }
        else if(other.tag.Equals("Ramp"))
        {
            print("ramp");
        }
        else
        {
            playerMovement.AirMovement();
        }
    }
}
