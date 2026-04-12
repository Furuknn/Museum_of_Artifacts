using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuseumEventManager : MonoBehaviour, IInteractable
{
    public List<Light> museumLights;
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
        greedLight.enabled = true;
        greedDoor.SetActive(false);
    }
}
