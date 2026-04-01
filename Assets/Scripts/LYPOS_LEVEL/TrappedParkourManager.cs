using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrappedParkourManager : MonoBehaviour
{
    public List<TrappedBlock> blocks;

    private void Start()
    {
        SelectSafeBlocks();
    }
    void SelectSafeBlocks()
    {
        int safe1 = Random.Range(0, 3);
        int safe2 = Random.Range(3, 6);
        int safe3 = Random.Range(6, 9);
        int safe4 = Random.Range(9, 12);

        blocks[safe1].isTrapped = false;
        blocks[safe2].isTrapped = false;
        blocks[safe3].isTrapped = false;
        blocks[safe4].isTrapped = false;
    }
}
