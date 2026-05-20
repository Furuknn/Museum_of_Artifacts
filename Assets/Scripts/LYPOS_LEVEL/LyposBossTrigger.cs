using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LyposBossTrigger : MonoBehaviour
{
    public LyposBoss boss;
    public GameObject doorCollider;
    public Transform doorLeft;
    public Transform doorRight;

    private void OnTriggerEnter(Collider other)
    {
        if (boss == null) return;
        if (boss._bossAwake == false)
        {
            doorCollider.SetActive(true);
            doorLeft.DOLocalRotate(new Vector3(90, 0, 0), 1.5f);
            doorRight.DOLocalRotate(new Vector3(90, 0, 0), 1.5f);
            boss.transform.DOLocalMoveY(1.35f, 4f).OnComplete(() => {
                boss.StartBoss();

            });

        }
    }
}
