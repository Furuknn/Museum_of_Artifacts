using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastlePuzzlePiece : MonoBehaviour, IInteractable
{
    public bool isObtained = false;

    public void Interact()
    {
        ParkourPiece parkour = GetComponent<ParkourPiece>();
        if (parkour != null) parkour.ParkourIslandEvent();
        if (!isObtained)
        {
            isObtained = true;
            gameObject.SetActive(false);
        }
    }
}
