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
    public bool isInteractable = true;
    public void Interact()
    {
        if (!isInteractable) return;
        StartCoroutine(PressButton());
    }

    public void InitializeButton(int no)
    {
        number = no;
        numberText.text = no.ToString();
    }

    public void OpenButton()
    {
        transform.DORotateQuaternion(Quaternion.Euler(0,0,0), 1f);
    }

    public void CloseButton()
    {
        transform.DORotateQuaternion(Quaternion.Euler(0, 0, 180), 1f);
    }
    IEnumerator PressButton()
    {
        OpenButton();
        yield return new WaitForSeconds(1f);
        FloorPuzzleManager.Instance.CheckButton(number);
    }
}
