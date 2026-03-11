using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorPuzzleManager : MonoBehaviour, IInteractable
{
    public static FloorPuzzleManager Instance;
    public GameObject resetButton;
    public List<FloorPuzzleButton> buttons;
    public List<int> pressedNumbers;
    public int buttonsNeedToPress;
    float enemiesToSpawn = 2;
    float enemiesMultiplier = 1;
    public List<GameObject> enemyGroups;
    public bool isDone = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }
    private void Start()
    {
        ShuffleButtons();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            //StartCoroutine(ShowButtons());
        }
    }

    public void Interact()
    {
        if (isDone) return;
        ShuffleButtons();
        StartCoroutine(ShowButtons());
    }

    IEnumerator ShowButtons()
    {
        foreach (var button in buttons)
        {
            button.OpenButton();
        }
        yield return new WaitForSeconds(3f);
        foreach (var button in buttons)
        {
            button.CloseButton();
        }
    }
    public void ShuffleButtons()
    {
        // 1. Buton sayýsý kadar bir sayý listesi oluþtur (1, 2, 3...)
        List<int> numbers = new List<int>();
        for (int i = 1; i <= buttons.Count; i++)
        {
            numbers.Add(i);
        }

        // 2. Sayý listesini rastgele karýþtýr (Fisher-Yates Algorithm)
        for (int i = 0; i < numbers.Count; i++)
        {
            int temp = numbers[i];
            int randomIndex = Random.Range(i, numbers.Count);
            numbers[i] = numbers[randomIndex];
            numbers[randomIndex] = temp;
        }

        // 3. Karýþtýrýlmýþ sayýlarý butonlara ata
        for (int i = 0; i < buttons.Count; i++)
        {
            buttons[i].InitializeButton(numbers[i]);

            // Görsel geri bildirim için buton üzerindeki yazýyý güncelleyebilirsin
            // buttons[i].UpdateUI(); 
        }

        Debug.Log("Buton numaralarý baþarýyla karýþtýrýldý!");
    }

    public void CheckButton(int x)
    {
        for (int i = 1; i < x; i++)
        {
            if (!pressedNumbers.Contains(i))
            {
                LoseEvent();
                return;
            }
        }
        pressedNumbers.Add(x);

        if (x == buttonsNeedToPress) WinEvent();
    }

    void LoseEvent()
    {
        Debug.LogWarning("FLOOR PUZZLE LOSE");
        pressedNumbers.Clear();
        foreach (var button in buttons)
        {
            button.CloseButton();
        }

        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        enemiesToSpawn = Mathf.Pow(enemiesToSpawn, enemiesMultiplier);

        enemyGroups[(int)enemiesMultiplier].SetActive(true);

        enemiesMultiplier++;
    }

    void WinEvent()
    {
        isDone = true;
        Debug.LogWarning("FLOOR PUZZLE WIN");
        foreach (var button in buttons)
        {
            if (pressedNumbers.Contains(button.number))
            {
                button.numberText.color = Color.green;
            }

            button.isInteractable = false;
        }
        resetButton.transform.DOMoveY(transform.position.y+1.5f, 4f);
    }
}
