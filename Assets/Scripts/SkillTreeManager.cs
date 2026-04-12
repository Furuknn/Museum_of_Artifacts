using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkillTreeManager : MonoBehaviour
{
    public static SkillTreeManager Instance;

    FlashlightStatsBase flashlightStatsRuntime;
    NightStickStatsBase nightStickStatsRuntime;

    [SerializeField] private GameObject nightStickTreePanel;
    [SerializeField] private GameObject flashlightTreePanel;
    [SerializeField] private GameObject taserTreePanel;

    [SerializeField] private List<AbilityPanelEntry> abilityPanels;

    private List<UpgradeDefinition> allUpgrades;
    private Dictionary<string, UpgradeDefinition> upgradeLookup;
    private static Dictionary<string, GameObject> abilityPanelLookup = new();

    public int skillPoints;

    public static System.Action<string> OnUpgradeUnlocked;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            //Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        abilityPanelLookup ??= new Dictionary<string, GameObject>();
    }

    private void Start()
    {
        if (WeaponStatsManager.Instance != null)
        {
            flashlightStatsRuntime = WeaponStatsManager.Instance.flashlightStatsRuntime;
            nightStickStatsRuntime = WeaponStatsManager.Instance.nightStickStatsRuntime;
        }

        allUpgrades = new List<UpgradeDefinition>
    {
        // --- FLASHLIGHT MAIN ---
        new("BeamUpgrade_01", 1, () => flashlightStatsRuntime.narrowDamage *= 1.15f),
        new("BeamUpgrade_02", 1, () => flashlightStatsRuntime.narrowSpeed *= 1.2f),
        new("BeamUpgrade_03", 2, () => {flashlightStatsRuntime.canDoubleDamage = true; flashlightStatsRuntime.doubleDamageChance=0.2f;}),
        new("BeamUpgrade_04", 1, () => flashlightStatsRuntime.narrowCooldown /= 1.1f),
        new("BeamUpgrade_05", 2, () => flashlightStatsRuntime.doubleDamageChance = 0.35f),
        new("BeamUpgrade_06", 2, () => Debug.Log("This has not been implemented yet")),
        new("BeamUpgrade_07", 3, () => Debug.Log("This has not been implemented yet")),
        new("BeamUpgrade_08", 1, () => Debug.Log("This has not been implemented yet")),

        // --- FLASHLIGHT WIDE ---
        new("WideUpgrade_01", 1, () => flashlightStatsRuntime.wideDamage *= 1.1f),
        new("WideUpgrade_02", 1, () => flashlightStatsRuntime.wideLifetime *= 1.2f),
        new("WideUpgrade_03", 2, () => flashlightStatsRuntime.wideExpansionMultiplier *= 1.15f),
        new("WideUpgrade_04", 1, () => Debug.Log("This has not been implemented yet")),
        new("WideUpgrade_05", 2, () => flashlightStatsRuntime.wideSpeed *= 1.1f),
        new("WideUpgrade_06", 2, () => flashlightStatsRuntime.wideCooldown *= 1.12f),
        new("WideUpgrade_07", 3, () => Debug.Log("This has not been implemented yet")),
        new("WideUpgrade_08", 1, () => Debug.Log("This has not been implemented yet")),

        // --- FLASHLIGHT BOMB ---
        new("BombUpgrade_01", 1, () => flashlightStatsRuntime.bombDamage *= 1.1f),
        new("BombUpgrade_02", 1, () => Debug.Log("This has not been implemented yet")),
        new("BombUpgrade_03", 2, () => flashlightStatsRuntime.bombAmount += 1),
        new("BombUpgrade_04", 1, () => flashlightStatsRuntime.bombLifetime *= 1.2f),
        new("BombUpgrade_05", 2, () => flashlightStatsRuntime.bombDamage *= 1.15f),
        new("BombUpgrade_06", 1, () => flashlightStatsRuntime.bombAmount += 1),
        new("BombUpgrade_07", 1, () => flashlightStatsRuntime.bombTick /= 1.08f),
        new("BombUpgrade_08", 1, () => Debug.Log("This has not been implemented yet")),

        // --- FLASHLIGHT ULTIMATE ---
        new("FlashUltUpgrade_01", 1, () => flashlightStatsRuntime.ultimateDamage *= 1.1f),
        new("FlashUltUpgrade_02", 1, () => flashlightStatsRuntime.ultimateCameraResistance /= 1.15f),
        new("FlashUltUpgrade_03", 2, () => flashlightStatsRuntime.ultimateWindUpTime /= 1.2f),
        new("FlashUltUpgrade_04", 1, () => flashlightStatsRuntime.ultimateHeaviness /= 1.3f),
        new("FlashUltUpgrade_05", 2, () => flashlightStatsRuntime.ultimateCooldown /= 1.18f),
        new("FlashUltUpgrade_06", 1, () => Debug.Log("This has not been implemented yet")),
        new("FlashUltUpgrade_07", 1, () => Debug.Log("This has not been implemented yet")),
        new("FlashUltUpgrade_08", 1, () => Debug.Log("This has not been implemented yet")),

        // --- NIGHTSTICK SMASHGROUND---
        new("SmashGroundUpgrade_01", 1, () => nightStickStatsRuntime.smashGroundDamage *= 1.25f),
        new("SmashGroundUpgrade_02", 1, () => nightStickStatsRuntime.smashGroundCooldown /= 1.2f),
        new("SmashGroundUpgrade_03", 2, () => nightStickStatsRuntime.smashGroundRadius *= 1.25f),
        new("SmashGroundUpgrade_04", 1, () => nightStickStatsRuntime.smashGroundStunTime *= 1.3f),
        new("SmashGroundUpgrade_05", 2, () => Debug.Log("This has not been implemented yet")),
        new("SmashGroundUpgrade_06", 1, () => Debug.Log("This has not been implemented yet")),
        new("SmashGroundUpgrade_07", 1, () => Debug.Log("This has not been implemented yet")),
        new("SmashGroundUpgrade_08", 1, () => Debug.Log("This has not been implemented yet")),

        // --- NIGHTSTICK SPIN---
        new("SpinUpgrade_01", 1, () => nightStickStatsRuntime.spinDuration *= 1.2f),
        new("SpinUpgrade_02", 1, () => nightStickStatsRuntime.spinSpeed *= 1.15f),
        new("SpinUpgrade_03", 2, () => nightStickStatsRuntime.spinPlayerSpeed *= 2f),
        new("SpinUpgrade_04", 1, () => nightStickStatsRuntime.spinHealthRegen = true),
        new("SpinUpgrade_05", 2, () => Debug.Log("This has not been implemented yet")),
        new("SpinUpgrade_06", 1, () => Debug.Log("This has not been implemented yet")),
        new("SpinUpgrade_07", 1, () => Debug.Log("This has not been implemented yet")),
        new("SpinUpgrade_08", 1, () => Debug.Log("This has not been implemented yet")),

        // --- NIGHTSTICK DASH---
        new("DashUpgrade_01", 1, () => nightStickStatsRuntime.dashDamage *= 1.3f),
        new("DashUpgrade_02", 1, () => nightStickStatsRuntime.dashCooldown /= 1.25f),
        new("DashUpgrade_03", 2, () => nightStickStatsRuntime.dashRange *= 1.4f),
        new("DashUpgrade_04", 1, () => nightStickStatsRuntime.dashImmunity = true),
        new("DashUpgrade_05", 2, () => Debug.Log("This has not been implemented yet")),
        new("DashUpgrade_06", 1, () => Debug.Log("This has not been implemented yet")),
        new("DashUpgrade_07", 1, () => Debug.Log("This has not been implemented yet")),
        new("DashUpgrade_08", 1, () => Debug.Log("This has not been implemented yet")),

        // --- NIGHTSTICK SHIELD---
        new("StickUltUpgrade_01", 1, () => nightStickStatsRuntime.shieldDuration *= 1.5f),
        new("StickUltUpgrade_02", 1, () => nightStickStatsRuntime.damageDeflect = true),
        new("StickUltUpgrade_03", 2, () => nightStickStatsRuntime.shieldCooldown /= 1.3f),
        new("StickUltUpgrade_04", 1, () => nightStickStatsRuntime.shieldSlowness = false),
        new("StickUltUpgrade_05", 2, () => Debug.Log("This has not been implemented yet")),
        new("StickUltUpgrade_06", 1, () => Debug.Log("This has not been implemented yet")),
        new("StickUltUpgrade_07", 1, () => Debug.Log("This has not been implemented yet")),
        new("StickUltUpgrade_08", 1, () => Debug.Log("This has not been implemented yet")),
    };


        // Build a lookup dictionary once for O(1) access
        upgradeLookup = allUpgrades.ToDictionary(u => u.name);

        foreach (var upgrade in allUpgrades)
            LoadUpgrade(upgrade.name);

        UpdateSkillTreeUI();

    }
    public static void RegisterAbilityPanel(string key, GameObject panel)
    {
        abilityPanelLookup[key] = panel;
        // Hide it by default on registration
        panel.SetActive(false);
        Debug.Log($"Registered ability panel: {key}");
    }

    private void OnEnable()
    {
        AbilitySelectionManager.OnAbilityConfirmed += OnAbilityConfirmed;
    }
    private void OnDisable()
    {
        AbilitySelectionManager.OnAbilityConfirmed -= OnAbilityConfirmed;
    }

    public void UpdateSkillTreeUI()
    {
        if (nightStickTreePanel != null) nightStickTreePanel.SetActive(false);
        if (flashlightTreePanel != null) flashlightTreePanel.SetActive(false);

        int currentHeroIndex = GameManager.Instance.currentHeroIndex;

        switch (currentHeroIndex)
        {
            case 0: // NightStick
                if (nightStickTreePanel != null) nightStickTreePanel.SetActive(true);
                break;

            case 1: // Flashlight
                if (flashlightTreePanel != null) flashlightTreePanel.SetActive(true);
                break;

            case 2: // Taser
                if (taserTreePanel != null) taserTreePanel.SetActive(true);
                break;

            default:
                Debug.LogWarning("SkillTreeManager: Invalid Character Index!");
                break;
        }

        if (AbilitySelectionManager.Instance == null)
        {
            Debug.LogWarning("AbilitySelectionManager is null!");
            return;
        }
        else
        {
            AbilitySelectionManager.Instance.SetActiveCharacter(currentHeroIndex);
        }
    }
    public void UpgradeStat(string upgradeName)
    {
        if (!upgradeLookup.TryGetValue(upgradeName, out var upgrade))
        {
            Debug.LogWarning($"No upgrade defined for: {upgradeName}");
            return;
        }

        if (skillPoints < upgrade.cost)
        {
            Debug.LogWarning($"Not enough skill points for {upgradeName}. Cost: {upgrade.cost}, Current: {skillPoints}");
            return;
        }

        if (PlayerPrefs.GetInt(upgradeName, 0) == 1)
        {
            Debug.LogWarning($"{upgradeName} is already unlocked.");
            return;
        }

        skillPoints -= upgrade.cost;
        upgrade.apply.Invoke();

        PlayerPrefs.SetInt(upgradeName, 1);
        PlayerPrefs.Save();

        OnUpgradeUnlocked?.Invoke(upgradeName);
    }
    // 1 = true, 0 = false


    private void LoadUpgrade(string upgradeName)
    {
        if (PlayerPrefs.GetInt(upgradeName, 0) == 1 && upgradeLookup.TryGetValue(upgradeName, out var upgrade))
            upgrade.apply.Invoke();
    }

    public void IncreaseSkillPoint(int increaseAmount)
    {
        skillPoints += increaseAmount;
    }

    private void OnAbilityConfirmed()
    {
        string selectedAbility = PlayerPrefs.GetString("SelectedAbility");

        foreach (var entry in abilityPanels)
            if (entry.panel != null) entry.panel.SetActive(false);

        var match = abilityPanels.Find(e => e.abilityKey == selectedAbility);
        if (match != null)
            match.panel.SetActive(true);
        else
            Debug.LogWarning($"No panel registered for ability: {selectedAbility}");
    }

    public UpgradeDefinition GetUpgrade(string name) =>
    upgradeLookup.TryGetValue(name, out var u) ? u : null;
}



[System.Serializable]
public class UpgradeDefinition
{
    public string name;
    public int cost;
    public System.Action apply;

    public UpgradeDefinition(string name, int cost, System.Action apply)
    {
        this.name = name;
        this.cost = cost;
        this.apply = apply;
    }
}

[System.Serializable]
public class AbilityPanelEntry
{
    public string abilityKey;
    public GameObject panel;
}