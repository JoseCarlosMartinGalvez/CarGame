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
        public GameObject wheelEffectGO;
        public ParticleSystem smokeParticle;
        public Axel axel;
    }

    public float maxAcceleration = 30.0f;
    public float brakeAcceleration = 50.0f;

    public float turnSensitivity = 1.5f;  // Ajustado para mejorar el giro
    public float maxSteerAngle = 30.0f;
    public float highSpeedSteerAngle = 15.0f;  // Ángulo de giro reducido a alta velocidad

    public List<Wheel> wheels;

    float moveInput;
    float steerInput;
    private float currentSpeed;

    private Rigidbody carRb;

    // Diccionarios para almacenar las posiciones y rotaciones iniciales locales de los modelos de las ruedas
    private Dictionary<WheelCollider, Vector3> initialWheelLocalPositions = new Dictionary<WheelCollider, Vector3>();
    private Dictionary<WheelCollider, Quaternion> initialWheelLocalRotations = new Dictionary<WheelCollider, Quaternion>();

    private void Start()
    {
        carRb = GetComponent<Rigidbody>();
        carRb.centerOfMass = new Vector3(0, -0.5f, 0);  // Bajar el centro de masa para mayor estabilidad
        StoreInitialWheelLocalTransforms();
        AdjustWheelFriction();
    }

    private void Update()
    {
        GetInputs();
        WheelEffects();
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
        currentSpeed = carRb.velocity.magnitude * 3.6f; // Convierte m/s a km/h

        foreach (var wheel in wheels)
        {
            wheel.wheelCollider.motorTorque = moveInput * 2400 * maxAcceleration * Time.deltaTime;
        }
    }

    void Steer()
    {
        foreach (var wheel in wheels)
        {
            if (wheel.axel == Axel.Front)
            {
                float targetSteerAngle = steerInput * turnSensitivity * maxSteerAngle;
                if (currentSpeed > 50f) // Reduce el ángulo de giro a alta velocidad
                {
                    targetSteerAngle = steerInput * turnSensitivity * highSpeedSteerAngle;
                }

                wheel.wheelCollider.steerAngle = Mathf.Lerp(wheel.wheelCollider.steerAngle, targetSteerAngle, 0.1f);
            }
        }
    }

    void Brake()
    {
        if (Input.GetKey(KeyCode.Space) || moveInput == 0)
        {
            foreach (var wheel in wheels)
            {
                wheel.wheelCollider.brakeTorque = 12000 * brakeAcceleration;
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

    void AdjustWheelFriction()
    {
        foreach (var wheel in wheels)
        {
            WheelFrictionCurve forwardFriction = wheel.wheelCollider.forwardFriction;
            WheelFrictionCurve sidewaysFriction = wheel.wheelCollider.sidewaysFriction;

            forwardFriction.stiffness = 3f;
            sidewaysFriction.stiffness = 4.0f;

            wheel.wheelCollider.forwardFriction = forwardFriction;
            wheel.wheelCollider.sidewaysFriction = sidewaysFriction;
        }
    }

    void WheelEffects()
    {
        foreach (var wheel in wheels)
        {
            if (Input.GetKey(KeyCode.Space) && wheel.axel == Axel.Rear && wheel.wheelCollider.isGrounded == true && carRb.velocity.magnitude >= 10.0f)
            {
                wheel.wheelEffectGO.GetComponentInChildren<TrailRenderer>().emitting = true;
                wheel.smokeParticle.Emit(1);
            }
            else
            {
                wheel.wheelEffectGO.GetComponentInChildren<TrailRenderer>().emitting = false;
            }
        }
    }
}
