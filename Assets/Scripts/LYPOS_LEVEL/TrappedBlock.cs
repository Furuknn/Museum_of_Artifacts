using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrappedBlock : MonoBehaviour
{
    public bool isTrapped = false;
    private void OnTriggerEnter(Collider other)
    {
        if (!isTrapped) return;

        ThirdPersonController player = other.GetComponent<ThirdPersonController>();
        if (player != null)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = false;
        }
    }
}
