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

    void Movement()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.position = new Vector3(transform.position.x * speed, transform.position.y, transform.position.z);
        }
    }
}
