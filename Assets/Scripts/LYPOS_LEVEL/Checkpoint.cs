using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int orderNo;
    public bool isClaimed = false;
    private void OnTriggerEnter(Collider other)
    {
        if (isClaimed) return;
        ThirdPersonController player = other.GetComponent<ThirdPersonController>();
        if (player != null)
        {
            isClaimed = true;
            CheckpointManager.Instance.ClaimCheckpoint(this);
        }
    }
}
