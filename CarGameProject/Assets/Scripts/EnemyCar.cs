using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class EnemyCar : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed;
    public float rotationSpeed;
    public int currentWaypoint = 0;

    // ROTATE WHEELS
    public Transform frontLeftWheel;
    public Transform frontRightWheel;
    public float wheelTurnAngle = 30f;

    public bool isCheckpoint = false;
    public float lapsToWin;
    public float laps = 0;

    public Transform Points;
    int currentPoint;
    public NavMeshAgent agent;

    // JUMP PARAMETERS
    public float jumpForce = 10f;
    public LayerMask jumpLayerMask; // Máscara de capa para detectar áreas de salto

    private Rigidbody rb;

    // Specific Rotation for Ramp
    public float rampRotationAngle = 45f; // Adjust the rotation angle for ramp

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        if (agent == null)
        {
            Debug.LogError("NavMeshAgent no encontrado en " + gameObject.name);
        }

        if (Points == null || Points.childCount == 0)
        {
            Debug.LogError("Points no asignado o no tiene hijos");
            return;
        }

        agent.SetDestination(Points.GetChild(currentPoint).position);

        // Disable gravity when using NavMeshAgent
        rb.useGravity = false;
    }

    private void Update()
    {
        if (agent == null || Points == null || Points.childCount == 0)
        {
            return;
        }

        if (agent.enabled)
        {
            agent.SetDestination(Points.GetChild(currentPoint).position);
        }

        if (Vector3.Distance(transform.position, Points.GetChild(currentPoint).position) < 20f)
        {
            currentPoint++;
            if (currentPoint >= Points.childCount) currentPoint = 0;
        }

        if (ShouldJump())
        {
            Jump();
        }

        if (laps >= lapsToWin)
        {
            SceneManager.LoadScene(3);
        }
    }

    bool ShouldJump()
    {
        // Detectar si el coche está sobre un área de salto
        NavMeshHit hit;
        if (agent.SamplePathPosition(NavMesh.AllAreas, 0.0f, out hit))
        {
            if ((jumpLayerMask.value & (1 << hit.mask)) != 0)
            {
                Debug.Log("Jump area detected");
                return true;
            }
        }
        return false;
    }

    void Jump()
    {
        // Realizar el salto
        agent.enabled = false; // Desactivar el agente durante el salto
        rb.useGravity = true; // Activar la gravedad durante el salto
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        // Esperar un momento antes de reactivar el agente
        Invoke("EnableNavMeshAgent", 1.0f); // Ajusta el tiempo según sea necesario
    }

    void EnableNavMeshAgent()
    {
        agent.enabled = true;
        rb.useGravity = false; // Desactivar la gravedad al reactivar el agente
        agent.SetDestination(Points.GetChild(currentPoint).position);
    }
}
