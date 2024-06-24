    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    EnemyCar enemyCar;
    CarScript carScript;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            carScript.laps++;
        }
        if (other.tag.Equals("Enemy"))
        {
            enemyCar.laps++;
        }
    }
}
