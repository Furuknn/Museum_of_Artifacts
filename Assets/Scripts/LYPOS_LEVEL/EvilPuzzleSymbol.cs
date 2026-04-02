using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvilPuzzleSymbol : MonoBehaviour,IInteractable
{
    public string evil;
    public Material mat;
    public bool isActive = false;
    public bool isInteractable = true;

    private void Awake()
    {
        mat = GetComponent<MeshRenderer>().material;
    }
    public void Interact()
    {
        if (!isInteractable) return;
        ToggleSymbol();
    }

    public void ToggleSymbol()
    {
        isActive = !isActive;
        if (isActive) mat.EnableKeyword("_EMISSION");
        else mat.DisableKeyword("_EMISSION");
        if (isActive) EvilPuzzleManager.Instance.SelectEvilSymbol(evil);
        else EvilPuzzleManager.Instance.DeselectEvilSymbol(evil);
    }

    public void DisableSymbol()
    {
        isActive = false;
        mat.DisableKeyword("_EMISSION");
        EvilPuzzleManager.Instance.DeselectEvilSymbol(evil);
    }
}
