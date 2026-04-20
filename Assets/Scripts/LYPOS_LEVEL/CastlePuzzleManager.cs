using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastlePuzzleManager : MonoBehaviour
{
    public static CastlePuzzleManager Instance;
    public List<CastlePuzzleSlot> slots = new List<CastlePuzzleSlot>();
    public List<MeshRenderer> meshes = new List<MeshRenderer>();
    public GameObject castleDoor;
    public Transform doorLeft;
    public Transform doorRight;
    public LyposBoss lypos;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        castleDoor.SetActive(true);
        //lypos = FindObjectOfType<LyposBoss>();
        
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
        //castleDoor.SetActive(false);
        transform.DOLocalMoveZ(-0.015f, 1f).OnComplete(() => {
            foreach (var mesh in meshes)
            {
                mesh.enabled = false;
            }
            doorLeft.DOLocalRotate(new Vector3(90, 0, 95), 4f);
            doorRight.DOLocalRotate(new Vector3(90, 0, -95), 4f);
        });
        
    }
}
