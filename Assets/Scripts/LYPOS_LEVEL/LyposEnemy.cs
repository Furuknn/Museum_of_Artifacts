using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LyposEnemy : MonoBehaviour
{
    public LyposEnemyType type;
    public void OnDie()
    {
        if (type == LyposEnemyType.Parkour && !ParkourPiece.Instance.isDone)
        {
            ParkourPiece.Instance.CheckEnemies();
        }
        if (type == LyposEnemyType.Evil && !EvilPuzzleManager.Instance.isDone)
        {
            EvilPuzzleManager.Instance.CheckEnemies();
        }
        
    }

    public enum LyposEnemyType
    {
        Parkour,
        Evil
    }
}
