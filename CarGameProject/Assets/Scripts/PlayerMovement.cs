using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //MOVEMENT
    int speedInt;
    float speed;
    float resistence;
    Animator anim;
    float movement;

    //LIFE
    float hearts;
    float damage;
    bool isDamaged;

    //Rigidbody
    Rigidbody rb;


    // Start is called before the first frame update
    void Start()
    {

        if (isDamaged)
        {
            hearts--;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    public void Movement()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.position = new Vector3(transform.position.x * speed, transform.position.y, transform.position.z);
        }
    }

    public void AirMovement()
    {
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }
}
