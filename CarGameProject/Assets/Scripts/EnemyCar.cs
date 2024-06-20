using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCar : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed;
    public float rotationSpeed;
    public int currentWaypoint = 0;

    //ROTATE WHEELS
    public Transform frontLeftWheel;
    public Transform frontRightWheel;
    public float wheelTurnAngle = 30f;

    private void Update()
    {
        if (waypoints.Length == 0) return;

        Vector3 targetPosition = waypoints[currentWaypoint].position;
        Vector3 direction = targetPosition - transform.position;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        }

        RotateFrontWheels(direction);
    }

    void RotateFrontWheels(Vector3 direction)
    {
        float angle = Mathf.Clamp(Vector3.SignedAngle(transform.forward, direction, Vector3.up), -wheelTurnAngle, wheelTurnAngle);

        frontLeftWheel.localRotation = Quaternion.Euler(0, 90+angle, 0);
        frontRightWheel.localRotation = Quaternion.Euler(0,90+angle, 0);
    }
}