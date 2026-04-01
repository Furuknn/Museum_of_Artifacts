using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        ThirdPersonController player = other.GetComponent<ThirdPersonController>();
        if (player != null)
        {
            CheckpointManager.Instance.RespawnPlayer();
        }
    }
}
