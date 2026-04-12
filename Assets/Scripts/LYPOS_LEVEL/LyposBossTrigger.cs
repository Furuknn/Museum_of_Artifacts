using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LyposBossTrigger : MonoBehaviour
{
    public LyposBoss boss;
    public GameObject door;

    private void OnTriggerEnter(Collider other)
    {
        if (boss == null) return;
        if (boss._bossAwake == false)
        {
            door.SetActive(true);
            boss.transform.DOLocalMoveY(3f, 4f).OnComplete(() => {
                boss.StartBoss();

            });

        }
    }
}
