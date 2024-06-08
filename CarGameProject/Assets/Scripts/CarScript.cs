using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CarScript : MonoBehaviour
{
    public enum Axel
    {
        Front,
        Rear
    }

    [SerializableAttribute]
    public struct Wheel
    {
        public GameObject wheelModel;
        public WheelCollider wheelCollider;
        public Axel axel;
    }

    public float maxAcceleration = 30.0f;
    public float brakeAcceleration = 50.0f;

    public List<Wheel> wheels;

    float moveInput;
    float steerInput;

    private Rigidbody carRb;

    private void Start()
    {
        carRb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        GetInputs();
    }

    private void LateUpdate()
    {
        Move();
    }

    void GetInputs()
    {
        moveInput = Input.GetAxis("Vertical");
    }
    void Move()
    {
        foreach(var wheel in wheels)
        {
            wheel.wheelCollider.motorTorque = moveInput * 600 * maxAcceleration * Time.deltaTime;
        }
    }
}
