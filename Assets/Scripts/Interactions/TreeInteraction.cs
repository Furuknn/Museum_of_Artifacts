using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeInteraction : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject apples;
    private bool _isInteractable = true;

    public bool isInteractable() => _isInteractable;
    public void Interact()
    {

        if (LevelManager.Instance != null)
        {
            Debug.Log($"Intreaction with tree and museum artifacts: {LevelManager.Instance.isMuseumArtifactsCursedCheck()}");
            if (!LevelManager.Instance.isMuseumArtifactsCursedCheck())
            {
                LevelManager.Instance.ArtifectsInMuseum();
                Debug.Log("Interaction: Agacla etkilesime gecildi: Artifactlar curselendi");

                if (!apples.activeSelf)
                    apples.SetActive(true);
            }

            UIManager.Instance.ToggleSkillTree();
            Debug.Log("Skill Tree açıldı");

            MuseumEventManager.Instance.OpenGreed();
        }


    }
}
