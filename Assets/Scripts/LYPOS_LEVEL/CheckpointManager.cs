using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;
    public List<Checkpoint> checkpoints;
    Checkpoint lastPoint;

    public Image respawnFade;
    private void Awake()
    {
        Instance = this;
    }

    public void ClaimCheckpoint(Checkpoint point)
    {
        if (lastPoint == null) lastPoint = point;
        else if (lastPoint.orderNo < point.orderNo)
        {
            lastPoint = point;
        }
    }

    public void RespawnPlayer()
    {
        if (lastPoint != null)
        {
            StartCoroutine(RespawnRoutine());
        }
    }

    IEnumerator RespawnRoutine()
    {
        Color fadeInColor = new Color(0, 0, 0, 1);
        if (respawnFade != null)
        {
            respawnFade.DOColor(fadeInColor, 1f);
        }
        yield return new WaitForSeconds(1f);
        ThirdPersonController.instance.characterController.enabled = false;
        ThirdPersonController.instance.transform.position = lastPoint.transform.position;
        ThirdPersonController.instance.characterController.enabled = true;
        Color fadeOutColor = new Color(0, 0, 0, 0);
        if (respawnFade != null)
        {
            respawnFade.DOColor(fadeOutColor, 1f);
        }
        
        List<IPlayerRespawn> respawns = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                                      .OfType<IPlayerRespawn>()
                                      .ToList();

        foreach (var respawn in respawns)
        {
            respawn.OnPlayerRespawn();
        }
    }
}

public interface IPlayerRespawn
{
    public void OnPlayerRespawn();
}
