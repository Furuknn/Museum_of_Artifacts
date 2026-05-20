using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardCountdown : MonoBehaviour
{
    public CardSO cardSO;
    [SerializeReference] public List<CardComponent> components = new List<CardComponent>();
    public bool actionEveryCount = false;
    public string value;
    public int count = 3;

    [Header("Visual Settings")]
    public Image countdownImage;
    public TextMeshProUGUI valueText;
    public List<GameObject> countdownSquares;

    public void InitializeCountdown()
    {
        components.Clear();
        components = cardSO.components;
        actionEveryCount = cardSO.actionEveryCount;
        count = cardSO.count;
        value = cardSO.value;
        if (cardSO.countdownSprite != null) countdownImage.sprite = cardSO.countdownSprite;
        valueText.text = value.ToString();
    }
    public void CountdownEvent()
    {
        if (actionEveryCount)
        {
            foreach (var comp in components)
            {
                comp.Use();
            }
        }
        else if (!actionEveryCount && count == 1)
        {
            foreach (var comp in components)
            {
                comp.Use();
            }
        }
        // Komponentleri çalýþtýr
        

        count--;

        // Görsel güncellenmesi (Index hatasý almamak için count'u kontrol et)
        if (count >= 0 && count < countdownSquares.Count)
        {
            int indexToDestroy = count; // Mevcut kare
            countdownSquares[indexToDestroy].GetComponent<Image>().DOColor(new Color(1, 1, 1, 0), 0.5f)
                .OnComplete(() => {
                    if (countdownSquares[indexToDestroy] != null) countdownSquares[indexToDestroy].SetActive(false);
                });
        }

        if (count <= 0) RemoveCountdown();
    }

    public void RemoveCountdown()
    {
        foreach (GameObject sqr in countdownSquares) sqr.SetActive(false);
        countdownImage.DOColor(new Color(1, 1, 1, 0), 1f).OnComplete(() => gameObject.SetActive(false));
    }

    private void OnDestroy()
    {
        // Hangi listede olduðunu biliyorsa kendini oradan sildirir
        if (CardGameManager.Instance != null)
        {
            if (CardGameManager.Instance.playerCountdowns.Contains(this.gameObject))
                CardGameManager.Instance.playerCountdowns.Remove(this.gameObject);

            else if (CardGameManager.Instance.dolvarisCountdowns.Contains(this.gameObject))
                CardGameManager.Instance.dolvarisCountdowns.Remove(this.gameObject);
        }
    }
}
