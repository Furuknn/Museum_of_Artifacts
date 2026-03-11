using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorPuzzleDeathTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        ThirdPersonController player = other.GetComponent<ThirdPersonController>();
        if (player != null)
        {
            Death();
        }
    }

    void Death()
    {

    }
}
