using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorPuzzleDoor : MonoBehaviour, IPlayerRespawn
{
    public bool isTrapped = true;
    public GameObject door1;
    public GameObject door2;
    public GameObject trap;
    bool isOpened = false;

    private void Start()
    {
        if (isTrapped) trap.SetActive(true);
    }
    private void OnTriggerEnter(Collider other)
    {
        ThirdPersonController player = other.GetComponent<ThirdPersonController>();
        if (player != null)
        {
            OpenDoor();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        ThirdPersonController player = other.GetComponent<ThirdPersonController>();
        if (player != null)
        {
            CloseDoor();
        }
    }

    public void OnPlayerRespawn()
    {
        CloseDoor();
    }

    void OpenDoor()
    {
        door1.transform.DOLocalRotate(new Vector3(0, 90, 0), 1f).SetEase(Ease.OutBounce);
        door2.transform.DOLocalRotate(new Vector3(0, -90, 0), 1f).SetEase(Ease.OutBounce);
        isOpened = true;
    }
    void CloseDoor()
    {
        door1.transform.DOLocalRotate(new Vector3(0, 0, 0), 1f).SetEase(Ease.OutBounce);
        door2.transform.DOLocalRotate(new Vector3(0, 0, 0), 1f).SetEase(Ease.OutBounce);
        isOpened = false;
    }
}
