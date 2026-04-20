using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuseumEventManager : MonoBehaviour, IInteractable
{
    public static MuseumEventManager Instance;
    public List<Light> museumLights;
    public GameObject treeWall;
    public Light greedLight;
    public Light lyposLight;
    public Light threnosLight;
    public Light dolvarisLight;
    public GameObject greedDoor;
    public GameObject lyposDoor;
    public GameObject threnosDoor;
    public GameObject dolvarisDoor;

    public bool callAnswered = false;
    public AudioSource ringSource;
    public AudioSource dolvarisSource;
    public List<AudioClip> dolvarisDialogues;
    private bool _isInteractable = true;

    public bool isInteractable() => _isInteractable;

    private void Awake()
    {
        if (Instance != this) Instance = this;
    }
    private void Start()
    {
        if (!callAnswered) CallEvent();
    }

    public void Interact()
    {
        if (!callAnswered) AnswerCall();
    }
    void CallEvent()
    {
        ringSource.loop = true;
        ringSource.Play();
    }

    void AnswerCall()
    {
        ringSource.Stop();
        callAnswered = true;
        //StartCoroutine(CallRoutine());
        EndCall();
    }

    IEnumerator CallRoutine()
    {
        dolvarisSource.PlayOneShot(dolvarisDialogues[0]);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        dolvarisSource.PlayOneShot(dolvarisDialogues[1]);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        dolvarisSource.PlayOneShot(dolvarisDialogues[2]);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        EndCall();
    }

    void EndCall()
    {
        foreach (Light light in museumLights)
        {
            light.enabled = false;
        }
        treeWall.SetActive(false);
    }

    public void OpenGreed()
    {
        greedLight.gameObject.SetActive(true);
        greedLight.enabled = true;
        greedDoor.SetActive(false);
    }

    public void TeleportPlayerToMuseum(string region)
    {
        var player = ThirdPersonController.Instance;
        GameObject spawnObj = GameObject.Find(region + "SpawnPoint");
        Vector3 targetPos = spawnObj.transform.position;

        LevelManager.Instance.ActiveMuseum(true);
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
        }

        player.transform.position = targetPos;

        if (cc != null)
        {
            cc.enabled = true;
        }
    }
}
