using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Accessibility;

public class CastlePuzzleSlot : MonoBehaviour, IInteractable
{
    public CastlePuzzlePiece truePiece;
    public GameObject visualPiece;
    public bool isActivated = false;
    private void Start()
    {
        visualPiece.SetActive(false);
    }
    public void Interact()
    {
        if (truePiece.isObtained)
        {
            visualPiece.SetActive(true);
            isActivated = true;
            CastlePuzzleManager.Instance.CheckPuzzle();
        }
    }
}
