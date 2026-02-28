using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using Unity.VisualScripting;
using Cinemachine;


public class SubLevelManager : MonoBehaviour
{
    public static SubLevelManager instace { get; private set; }
    [System.Serializable]
    public class SpawnEnemy
    {
        public string name;
        public NavMeshModifierVolume[] spawnArea;
        public GameObject[] Enemies;
        public int amountOfSpawn;

    }
    [SerializeField] private List<SpawnEnemy> enemies;
    [SerializeField] private List<GameObject> allEnemyList = new List<GameObject>();

    [Header ("Boss Trigger")]
    [SerializeField] private Collider bossTrigerCollider;
    void Awake()
    {
        if (instace == null) instace = this;
        SpawnEnemies();
    }
    private void SpawnEnemies()
    {
        foreach (SpawnEnemy enemyGroup in enemies)
        {
            for (int i = 0; i < enemyGroup.amountOfSpawn; i++)
            {
                int randomAreaIndex = Random.Range(0, enemyGroup.spawnArea.Length);
                int randomEnemyIndex = Random.Range(0, enemyGroup.Enemies.Length);

                NavMeshModifierVolume selectedVolume = enemyGroup.spawnArea[randomAreaIndex];
                GameObject selectedEnemyPrefab = enemyGroup.Enemies[randomEnemyIndex];
                Vector3 halfSize = selectedVolume.size / 2f;

                Vector3 randomLocalPosition = selectedVolume.center + new Vector3(
                    Random.Range(-halfSize.x, halfSize.x),
                    0,
                    Random.Range(-halfSize.z, halfSize.z)
                );

                Vector3 randomWorldPoint = selectedVolume.transform.TransformPoint(randomLocalPosition);

                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomWorldPoint, out hit, 5f, NavMesh.AllAreas))
                {
                    GameObject newEnemy = Instantiate(selectedEnemyPrefab, hit.position, Quaternion.identity, transform);
                    allEnemyList.Add(newEnemy);
                }

            }
        }
    }

    public void CheckEnemyList(GameObject enemyGO)
    {
        if (allEnemyList.Contains(enemyGO))
        {
            allEnemyList.Remove(enemyGO);
        }
        //IF THERE WILL BE 1 ENEMY MAYBE ADD SOME FEEDBACK SOUND OR TEXT
        if (allEnemyList.Count == 0)
        {
            Debug.LogWarning("BİTTİ");

            //There will be a system of tp to level's boss
            /*LevelManager.instance.ReturnFromLevel();
            LevelManager.instance.DestroyCurrentLevel();
            LevelManager.instance.ReturnWithWinFromLevel();*/
        }


    }
}
