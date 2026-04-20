using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorPuzzleManager : MonoBehaviour, IPlayerRespawn
{
    public List<DoorPuzzleDoor> doors;
    public List<DoorHintGroup> doubleHints;
    public List<DoorHintGroup> tripleHints;
    public List<DoorHintGroup> quadraHints;
    public List<DoorHintGroup> pentaHints;

    private void Start()
    {
        SelectDoors();
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

    void SelectDoors()
    {
        foreach (var door in doors)
        {
            door.ToggleTrap(true);
        }

        DoorHintGroup doubleDoor = doubleHints[Random.Range(0, doubleHints.Count)];

        doors[0].hint.text = doubleDoor.doors[0];
        doors[1].hint.text = doubleDoor.doors[1];
        doors[doubleDoor.trueDoorIndex].ToggleTrap(false);

        DoorHintGroup tripleDoor = tripleHints[Random.Range(0, tripleHints.Count)];

        doors[2].hint.text = tripleDoor.doors[0];
        doors[3].hint.text = tripleDoor.doors[1];
        doors[4].hint.text = tripleDoor.doors[2];
        doors[tripleDoor.trueDoorIndex + 2].ToggleTrap(false);

        DoorHintGroup quadraDoor = quadraHints[Random.Range(0, quadraHints.Count)];

        doors[5].hint.text = quadraDoor.doors[0];
        doors[6].hint.text = quadraDoor.doors[1];
        doors[7].hint.text = quadraDoor.doors[2];
        doors[8].hint.text = quadraDoor.doors[3];
        doors[quadraDoor.trueDoorIndex + 5].ToggleTrap(false);

        DoorHintGroup pentaDoor = pentaHints[Random.Range(0, pentaHints.Count)];

        doors[9].hint.text = pentaDoor.doors[0];
        doors[10].hint.text = pentaDoor.doors[1];
        doors[11].hint.text = pentaDoor.doors[2];
        doors[12].hint.text = pentaDoor.doors[3];
        doors[13].hint.text = pentaDoor.doors[4];
        doors[pentaDoor.trueDoorIndex + 9].ToggleTrap(false);
    }

    public void OnPlayerRespawn()
    {
        SelectDoors();
    }

    [System.Serializable]
    public struct DoorHintGroup
    {
        public int trueDoorIndex;
        public List<string> doors;
    }
}
