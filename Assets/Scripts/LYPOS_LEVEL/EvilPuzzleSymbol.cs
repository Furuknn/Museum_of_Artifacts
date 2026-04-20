using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvilPuzzleSymbol : MonoBehaviour,IInteractable
{
    public string evil;
    public Material mat;
    public List<Material> mats;
    public bool isActive = false;
    public bool _isInteractable = true;

    public bool isInteractable() => _isInteractable;

    private void Awake()
    {
        GetComponent<MeshRenderer>().GetSharedMaterials(mats);
        mat = mats[1];
    }
    public void Interact()
    {
        if (!_isInteractable) return;
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
