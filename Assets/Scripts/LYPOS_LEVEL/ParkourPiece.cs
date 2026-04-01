using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG;
using DG.Tweening;

public class ParkourPiece : MonoBehaviour
{
    public static ParkourPiece Instance;
    public Transform enemiesParent;
    public Transform walls;
    public int enemyCount;
    public bool isDone = false;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        enemiesParent.gameObject.SetActive(false);
    }
    public void ParkourIslandEvent()
    {
        if (isDone) return;
        enemiesParent.gameObject.SetActive(true);
        walls.DOMoveY(walls.transform.position.y + 7f, 4f);
    }

    public void CheckEnemies()
    {
        if (isDone) return;
        enemyCount--;
        if (enemyCount == 0)
        {
            walls.DOMoveY(0f, 4f);
            isDone = true;
        }
    }
}
