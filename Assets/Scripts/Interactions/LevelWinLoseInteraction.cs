using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelWinLoseInteraction : MonoBehaviour, IInteractable
{   
    [SerializeField] private bool isWin;
    public void Interact()
    {
        LevelManager.Instance.ReturnFromLevel();
        LevelManager.Instance.DestroyCurrentLevel();

        if (isWin)
        {
            LevelManager.Instance.ReturnWithWinFromLevel();
        }
        else
        {
            LevelManager.Instance.ReturnWithLoseFromLevel();
        }
    }

    
}
