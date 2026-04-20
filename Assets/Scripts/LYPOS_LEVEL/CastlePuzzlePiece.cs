using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastlePuzzlePiece : MonoBehaviour, IInteractable
{
    public bool isObtained = false;
    private bool _isInteractable = true;

    public bool isInteractable() => _isInteractable;

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
