using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeInteraction : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject apples;
    public void Interact()
    {

        if (LevelManager.instance != null)
        {
            Debug.Log($"Intreaction with tree and museum artifacts: {LevelManager.instance.isMuseumArtifactsCursedCheck()}");
            if (!LevelManager.instance.isMuseumArtifactsCursedCheck())
            {
                LevelManager.instance.ArtifectsInMuseum();
                Debug.Log("Interaction: Agacla etkilesime gecildi: Artifactlar curselendi");

                if (!apples.activeSelf)
                    apples.SetActive(true);
            }

            UIManager.instance.ToggleSkillTree();
            Debug.Log("Skill Tree açıldı");
        }


    }
}
