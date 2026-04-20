using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG;
using DG.Tweening;

public class FloorPuzzleButton : MonoBehaviour, IInteractable
{
    public TextMeshPro numberText;
    public int number;
    public bool _isInteractable = true;

    public bool isInteractable() => _isInteractable;
    public void Interact()
    {
        if (!_isInteractable) return;
        StartCoroutine(PressButton());
    }

    public void InitializeButton(int no)
    {
        number = no;
        numberText.text = no.ToString();
    }

    public void OpenButton()
    {
        transform.DOLocalRotate(new Vector3(180,0,0), 1f);
    }

    public void CloseButton()
    {
        transform.DOLocalRotate(new Vector3(0, 0, 0), 1f);
    }
    IEnumerator PressButton()
    {
        OpenButton();
        yield return new WaitForSeconds(1f);
        FloorPuzzleManager.Instance.CheckButton(number);
    }
}
