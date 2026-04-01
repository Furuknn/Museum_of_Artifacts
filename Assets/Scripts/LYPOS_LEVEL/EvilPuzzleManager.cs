using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvilPuzzleManager : MonoBehaviour, IInteractable
{
    public static EvilPuzzleManager Instance;
    public GameObject door;
    public GameObject enemies;
    public Transform enemyParent;
    public List<EvilPuzzleSymbol> symbols;
    public List<GameObject> greedObjects;
    public List<GameObject> sorrowObjects;
    public List<GameObject> warObjects;
    public List<GameObject> deceptionObjects;

    public List<string> evils = new List<string>(4);
    public List<string> chosenEvils = new List<string>(2);
    public List<string> playersChoose;

    GameObject firstObject;
    GameObject secondObject;
    public int enemyCount;
    bool isInteractable = true;
    public bool isDone = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    private void Start()
    {
        playersChoose.Clear();
        ChooseEvils();
    }

    public void Interact()
    {
        if (!isInteractable || isDone) return;
        CheckEvils();
    }

    public void SelectEvilSymbol(string evil)
    {
        if (playersChoose.Count < 2)
        {
            playersChoose.Add(evil);
        }
        else
        {
            playersChoose.Add(evil);
            foreach (EvilPuzzleSymbol symbol in symbols)
            {
                if (symbol.evil == playersChoose[0])
                {
                    symbol.DisableSymbol();
                    return;
                }
            }
        }
    }

    public void DeselectEvilSymbol(string evil)
    {
        if (playersChoose.Contains(evil)) playersChoose.Remove(evil);
    }

    public void CheckEnemies()
    {
        enemyCount--;
        if (enemyCount == 0) ChooseEvils();
    }
    void ChooseEvils()
    {
        playersChoose.Clear();
        if (chosenEvils.Count > 0)
        {
            evils.Add(chosenEvils[0]);
            evils.Add(chosenEvils[1]);
        }
        chosenEvils.Clear();
        chosenEvils.Add(evils[Random.Range(0, evils.Count)]);
        evils.Remove(chosenEvils[0]);
        chosenEvils.Add(evils[Random.Range(0, evils.Count)]);
        evils.Remove(chosenEvils[1]);

        if (chosenEvils[0] == "Greed")
        {
            firstObject = greedObjects[Random.Range(0, greedObjects.Count)];
            firstObject.SetActive(true);
        }
        else if (chosenEvils[0] == "Sorrow")
        {
            firstObject = sorrowObjects[Random.Range(0, sorrowObjects.Count)];
            firstObject.SetActive(true);
        }
        else if (chosenEvils[0] == "War")
        {
            firstObject = warObjects[Random.Range(0, warObjects.Count)];
            firstObject.SetActive(true);
        }
        else if (chosenEvils[0] == "Deception")
        {
            firstObject = deceptionObjects[Random.Range(0, deceptionObjects.Count)];
            firstObject.SetActive(true);
        }

        if (chosenEvils[1] == "Greed")
        {
            secondObject = greedObjects[Random.Range(0, greedObjects.Count)];
            secondObject.SetActive(true);
        }
        else if (chosenEvils[1] == "Sorrow")
        {
            secondObject = sorrowObjects[Random.Range(0, sorrowObjects.Count)];
            secondObject.SetActive(true);
        }
        else if (chosenEvils[1] == "War")
        {
            secondObject = warObjects[Random.Range(0, warObjects.Count)];
            secondObject.SetActive(true);
        }
        else if (chosenEvils[1] == "Deception")
        {
            secondObject = deceptionObjects[Random.Range(0, deceptionObjects.Count)];
            secondObject.SetActive(true);
        }

        if (enemyParent.transform.childCount > 0) Destroy(enemyParent.transform.GetChild(0).gameObject);
        Instantiate(enemies, enemyParent);
        enemyParent.gameObject.SetActive(false);
        isInteractable = true;

        enemyCount = enemies.transform.childCount;
        foreach (EvilPuzzleSymbol symbol in symbols)
        {
            symbol.DisableSymbol();
            symbol.isInteractable = true;
        }
    }
    public void CheckEvils()
    {
        if (playersChoose.Count < 2) return;

        foreach (string x in playersChoose)
        {
            if (!chosenEvils.Contains(x))
            {
                LoseEvent();
                return;
            }
        }

        WinEvent();
    }

    void LoseEvent()
    {
        isInteractable = false;
        enemyParent.gameObject.SetActive(true);
        foreach (EvilPuzzleSymbol symbol in symbols)
        {
            symbol.DisableSymbol();
            symbol.isInteractable = false;
        }

        firstObject.SetActive(false);
        secondObject.SetActive(false);
    }

    void WinEvent()
    {
        door.SetActive(false);
        isDone = true;
    }
}
