using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerXpManagement : MonoBehaviour
{
    public static PlayerXpManagement Instance { get; private set; }
    [SerializeField] private float playerXp;
    [SerializeField] private float xpMultiplier;
    [SerializeField] private float xpReqToLvlUp;
    [SerializeField] private int levelPoint;
    [SerializeField] private Image inGameXpBarUI;
    [SerializeField] private TextMeshProUGUI inGameLevelPointUI;
    [SerializeField] private float reduceSpeed = 2f;

    private int increaseAmount = 1;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        levelPoint = 0;
       
    }

    private void Start()
    {
        inGameXpBarUI = UIManager.Instance.GetInGameXpBar();
        inGameLevelPointUI = UIManager.Instance.GetInGameLevelPointText();
        Debug.Log($"xpBar UI: {inGameXpBarUI.gameObject.name}");
        xpReqToLvlUp = GameManager.Instance.characters[GameManager.Instance.currentHeroIndex].playerStatisticsSO.xpRequiredToLevelUp;
        xpMultiplier = GameManager.Instance.characters[GameManager.Instance.currentHeroIndex].playerStatisticsSO.xpMultiplier;
        Debug.Log($"XP required to level up: {xpReqToLvlUp}");
        UpdateXpUI();
    }

    // Update is called once per frame
    void Update()
    {
        //FOR TESTING
        if (Input.GetMouseButtonDown((int)MouseButton.Middle))
        {
            ModifyXp(10);
        }
        UpdateXpUI();
    }
    private void UpdateXpUI()
    {
        inGameXpBarUI.fillAmount = Mathf.MoveTowards(inGameXpBarUI.fillAmount, playerXp / xpReqToLvlUp, reduceSpeed * Time.deltaTime);
        inGameLevelPointUI.text = $"Level Point: {levelPoint}";
    }
    public void ModifyXp(float amount)
    {
        Debug.Log($"ModifyXp(): Player gained some xp: {amount}");
        playerXp += amount * xpMultiplier;

        //Increases player's level
        if (playerXp >= xpReqToLvlUp)
        {
            levelPoint++;
            playerXp -= xpReqToLvlUp;

            SkillTreeManager.Instance.IncreaseSkillPoint(increaseAmount);
        }
    }
    public void ResetXp()
    {
        playerXp = 0;
        levelPoint = 0;
    }

    public int GetLevelPoint()
    {
        return levelPoint;
    }
}
