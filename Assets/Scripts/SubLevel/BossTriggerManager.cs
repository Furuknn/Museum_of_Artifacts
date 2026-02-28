using UnityEngine;

public class BossTriggerManager : MonoBehaviour
{
    [Header("Boss")]
    [SerializeField] private GameObject bossPrefab;
    [SerializeField] private Transform bossSpawnPosition;
    [SerializeField, Tooltip ("Kullandıktan sonra trigger yok olacak mı?")] private bool afterUseDestroy = false;
    [SerializeField] private Animator anim;

    private bool bossHasSpawned = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !bossHasSpawned)
        {
            SpawnBoss();
        }
        
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SpawnBoss();
        }
    }

    private void SpawnBoss()
    {
        anim.SetTrigger("closeTheGates");
        bossHasSpawned=true;
        Debug.Log("Boss Spawn Edildi!");
        Instantiate(bossPrefab, bossSpawnPosition.position, bossSpawnPosition.rotation);
        //Instantiate(spawnParticles, spawnPoint.position, Quaternion.identity); Maybe we will use

        if (afterUseDestroy)
        {
            Destroy(gameObject);
        }
    }
}
