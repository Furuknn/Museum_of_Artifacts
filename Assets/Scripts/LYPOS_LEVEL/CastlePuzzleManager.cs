using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastlePuzzleManager : MonoBehaviour
{
    public static CastlePuzzleManager Instance;
    public List<CastlePuzzleSlot> slots = new List<CastlePuzzleSlot>();
    public GameObject castleDoor;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        castleDoor.SetActive(true);
    }

    public void CheckPuzzle()
    {
        foreach (var slot in slots)
        {
            if (!slot.isActivated) return;
        }

        Invoke(nameof(OpenCastleDoor), 2f);
    }

    public void OpenCastleDoor()
    {
        castleDoor.SetActive(false);
    }
}
