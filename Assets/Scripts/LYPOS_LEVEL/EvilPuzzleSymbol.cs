using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvilPuzzleSymbol : MonoBehaviour,IInteractable
{
    public string evil;
    public GameObject indicator;
    public bool isActive = false;
    public bool isInteractable = true;
    public void Interact()
    {
        if (!isInteractable) return;
        ToggleSymbol();
    }

    public void ToggleSymbol()
    {
        isActive = !isActive;
        indicator.SetActive(isActive);
        if (isActive) EvilPuzzleManager.Instance.SelectEvilSymbol(evil);
        else EvilPuzzleManager.Instance.DeselectEvilSymbol(evil);
    }

    public void DisableSymbol()
    {
        isActive = false;
        indicator.SetActive(isActive);
        EvilPuzzleManager.Instance.DeselectEvilSymbol(evil);
    }
}
