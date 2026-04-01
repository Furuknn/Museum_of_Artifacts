using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DoorPuzzleManager : MonoBehaviour, IPlayerRespawn
{
    public List<DoorPuzzleDoor> doors;

    private void Start()
    {
        ShuffleDoors();
    }
    void ShuffleDoors()
    {
        foreach (var door in doors)
        {
            door.ToggleTrap(true);
        }
        int firstDoor = Random.Range(0, 2);
        int secondDoor = Random.Range(2, 5);
        int thirdDoor = Random.Range(5, 9);
        int fourthDoor = Random.Range(9, 14);

        doors[firstDoor].ToggleTrap(false);
        doors[secondDoor].ToggleTrap(false);
        doors[thirdDoor].ToggleTrap(false);
        doors[fourthDoor].ToggleTrap(false);
    }

    public void OnPlayerRespawn()
    {
        ShuffleDoors();
    }
}
