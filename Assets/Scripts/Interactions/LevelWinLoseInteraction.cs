using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelWinLoseInteraction : MonoBehaviour, IInteractable
{   
    [SerializeField] private bool isWin;
    public void Interact()
    {
        LevelManager.instance.ReturnFromLevel();
        LevelManager.instance.DestroyCurrentLevel();

        if (isWin)
        {
            LevelManager.instance.ReturnWithWinFromLevel();
        }
        else
        {
            LevelManager.instance.ReturnWithLoseFromLevel();
        }
    }

    
}
