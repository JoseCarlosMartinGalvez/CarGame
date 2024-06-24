using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

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

    public bool isCheckpoint;
    public float lapsToWin;
    public float laps;
    public TextMeshProUGUI lapsText;

    isGrounded isGroundedScript;

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
        lapsText.text = laps.ToString() + "/1";
    }

    private void Update()
    {
        GetInputs();
    }

    private void LateUpdate()
    {
        Move();
        Brake();
    }

    private void FixedUpdate()
    {
        Steer();
        AdjustWheelFriction();
        UpdateWheelAnimations();
        WheelEffects();
    }

    void GetInputs()
    {
        moveInput = Input.GetAxis("Vertical");
        steerInput = Input.GetAxis("Horizontal");
    }

    public void AirMovement()
    {
        carRb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Move()
    {
        // Calcula la velocidad actual en km/h
        currentSpeed = carRb.velocity.magnitude * 3.6f; // Convierte m/s a km/h

        // Limita la velocidad máxima
        float maxSpeed = 150f; // Velocidad máxima en km/h
        if (currentSpeed > maxSpeed)
        {
            currentSpeed = maxSpeed;
        }

        foreach (var wheel in wheels)
        {
            // Ajusta el torque aplicado a las ruedas
            float torque = moveInput * 2400 * maxAcceleration * Time.deltaTime;
            wheel.wheelCollider.motorTorque = torque;
        }
    }


    void Steer()
    {
        foreach (var wheel in wheels)
        {
            if (wheel.axel == Axel.Front)
            {
                float targetSteerAngle = steerInput * turnSensitivity * maxSteerAngle;

                if (currentSpeed > 20f)
                {
                    float speedFactor = Mathf.InverseLerp(20f, 50f, currentSpeed);
                    float adjustedSteerAngle = Mathf.Lerp(maxSteerAngle, highSpeedSteerAngle, speedFactor);
                    targetSteerAngle = steerInput * turnSensitivity * adjustedSteerAngle;
                }

                float interpolationFactor = Mathf.Lerp(0.1f, 0.05f, Mathf.InverseLerp(0f, 50f, currentSpeed));

                wheel.wheelCollider.steerAngle = Mathf.Lerp(wheel.wheelCollider.steerAngle, targetSteerAngle, interpolationFactor);
            }
        }
    }

    void AdjustWheelFriction()
    {
        foreach (var wheel in wheels)
        {
            WheelFrictionCurve forwardFriction = wheel.wheelCollider.forwardFriction;
            forwardFriction.stiffness = Mathf.Lerp(1f, 2f, Mathf.InverseLerp(0f, 50f, currentSpeed));
            wheel.wheelCollider.forwardFriction = forwardFriction;

            WheelFrictionCurve sidewaysFriction = wheel.wheelCollider.sidewaysFriction;
            sidewaysFriction.stiffness = Mathf.Lerp(1f, 4f, Mathf.InverseLerp(0f, 50f, currentSpeed));
            wheel.wheelCollider.sidewaysFriction = sidewaysFriction;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CheckPoint"))
        {
            isCheckpoint = true;
        }
        if (other.CompareTag("FinishLine") && isCheckpoint == true)
        {
            laps++;
            if (laps >= lapsToWin)
            {
                SceneManager.LoadScene(2);
            }
            lapsText.text = laps.ToString() + "/1";
            isCheckpoint = false;
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
