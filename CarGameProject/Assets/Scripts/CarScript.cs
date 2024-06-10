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

    [Serializable]
    public struct Wheel
    {
        public GameObject wheelModel;
        public WheelCollider wheelCollider;
        public Axel axel;
    }

    public float maxAcceleration = 30.0f;
    public float brakeAcceleration = 50.0f;

    public float turnSensitivity = 1.0f;
    public float maxSteerAngle = 30.0f;

    public List<Wheel> wheels;

    float moveInput;
    float steerInput;

    private Rigidbody carRb;

    // Diccionarios para almacenar las posiciones y rotaciones iniciales locales de los modelos de las ruedas
    private Dictionary<WheelCollider, Vector3> initialWheelLocalPositions = new Dictionary<WheelCollider, Vector3>();
    private Dictionary<WheelCollider, Quaternion> initialWheelLocalRotations = new Dictionary<WheelCollider, Quaternion>();

    private void Start()
    {
        carRb = GetComponent<Rigidbody>();
        StoreInitialWheelLocalTransforms();
    }

    private void Update()
    {
        GetInputs();
    }

    private void LateUpdate()
    {
        Move();
        Steer();
        Brake();
        UpdateWheelAnimations();
    }

    void GetInputs()
    {
        moveInput = Input.GetAxis("Vertical");
        steerInput = Input.GetAxis("Horizontal");
    }

    void Move()
    {
        foreach (var wheel in wheels)
        {
            wheel.wheelCollider.motorTorque = moveInput * 600 * maxAcceleration * Time.deltaTime;
        }
    }

    void Steer()
    {
        foreach (var wheel in wheels)
        {
            if (wheel.axel == Axel.Front)
            {
                var _steerAngle = steerInput * turnSensitivity * maxSteerAngle;
                wheel.wheelCollider.steerAngle = Mathf.Lerp(wheel.wheelCollider.steerAngle, _steerAngle, 0.6f);
            }
        }
    }

    void Brake()
    {
        if (Input.GetKey(KeyCode.Space) || moveInput == 0)
        {
            foreach (var wheel in wheels)
            {
                wheel.wheelCollider.brakeTorque = 300 * brakeAcceleration * Time.deltaTime;
            }
        }
        else
        {
            foreach (var wheel in wheels)
            {
                wheel.wheelCollider.brakeTorque = 0;
            }
        }
    }

    void UpdateWheelAnimations()
    {
        foreach (var wheel in wheels)
        {
            Quaternion rot;
            Vector3 pos;
            wheel.wheelCollider.GetWorldPose(out pos, out rot);

            // Actualiza la posición y rotación del modelo de la rueda
            wheel.wheelModel.transform.position = pos;

            // Ajusta la rotación del modelo de la rueda para que sea correcta
            wheel.wheelModel.transform.rotation = rot * initialWheelLocalRotations[wheel.wheelCollider];
        }
    }

    void StoreInitialWheelLocalTransforms()
    {
        foreach (var wheel in wheels)
        {
            initialWheelLocalPositions[wheel.wheelCollider] = wheel.wheelModel.transform.localPosition;
            initialWheelLocalRotations[wheel.wheelCollider] = wheel.wheelModel.transform.localRotation;
        }
    }
}
