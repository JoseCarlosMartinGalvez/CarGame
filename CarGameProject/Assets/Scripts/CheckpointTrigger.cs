using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    CarScript carScript;
    EnemyCar enemyCar;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            carScript = other.GetComponent<CarScript>();
            if (carScript != null)
            {
                carScript.isCheckpoint = true;
            }
        }
        if (other.CompareTag("Enemy"))
        {
            enemyCar = other.GetComponent<EnemyCar>();
            if (carScript != null)
            {
                carScript.isCheckpoint = true;
            }
        }
    }
}
