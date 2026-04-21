using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MuseumPortal : MonoBehaviour, IInteractable
{
    public string museumName;
    public MuseumSpawnRegions region;
    private bool _isInteractable = true;

    public bool isInteractable() => _isInteractable;

    public void Interact()
    {
        StartCoroutine(TeleportToMuseum());
    }

    IEnumerator TeleportToMuseum()
    {
        AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        LevelManager.Instance.ModifyCurrentLevelName(museumName);
        MuseumEventManager.Instance.TeleportPlayerToMuseum(region.ToString());
        while (!asyncOperation.isDone)
        {
            yield return null;
        }

        Scene newlyLoadedScene = SceneManager.GetSceneByName(museumName);

        if (newlyLoadedScene.IsValid())
        {
            // Bu sat�r, Lighting (Fog dahil) ayarlar�n�n bu sahneden al�nmas�n� sa�lar
            SceneManager.SetActiveScene(newlyLoadedScene);
        }

        
    }

    public enum MuseumSpawnRegions
    {
        Greed,
        Lypos,
        Threnos,
        Dolvaris
    }
}
