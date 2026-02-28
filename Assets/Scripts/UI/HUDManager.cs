using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [SerializeField] private List<Image> cooldownIcons;
    [SerializeField] private List<TextMeshProUGUI> cooldownTexts;

    private Dictionary<int, Coroutine> cooldownRoutines = new();


    private void Start()
    {
        foreach(Image icon in cooldownIcons)
        {
            icon.fillAmount = 0;
        }

        foreach(TextMeshProUGUI text in cooldownTexts)
        {
            text.text=string.Empty;
        }
    }

    // attackIndex:
    // 0 = Base
    // 1 = Secondary
    // 2 = Ultimate
    public void StartCooldown(float cooldown, int attackIndex)
    {
        // Stop previous cooldown if any
        if (cooldownRoutines.ContainsKey(attackIndex))
        {
            StopCoroutine(cooldownRoutines[attackIndex]);
        }

        cooldownRoutines[attackIndex] =
            StartCoroutine(CooldownRoutine(cooldown, attackIndex));
    }

    private IEnumerator CooldownRoutine(float cooldown, int attackIndex)
    {
        Image icon = cooldownIcons[attackIndex];
        TextMeshProUGUI text = cooldownTexts[attackIndex];

        float timer = 0f;

        icon.fillAmount = 1f;

        while (timer < cooldown)
        {
            timer += Time.deltaTime;

            float remaining = cooldown - timer;
            icon.fillAmount = remaining / cooldown;

            int secondsLeft = Mathf.CeilToInt(remaining);
            text.text = secondsLeft.ToString();

            yield return null;
        }

        icon.fillAmount = 0f;
        text.text = string.Empty;
    }
}
