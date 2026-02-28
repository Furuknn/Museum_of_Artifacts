using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStatsManager : MonoBehaviour
{
    public static WeaponStatsManager Instance { get; private set; }

    [SerializeField] private FlashlightStatsBase flashlightStatsBase;

    public FlashlightStatsBase flashlightStatsRuntime;

    [Header("NightStick")]
    [SerializeField] private NightStickStatsBase nightStickStatsBase;
    public NightStickStatsBase nightStickStatsRuntime;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);


        if (flashlightStatsBase != null)
        {
            flashlightStatsRuntime = Instantiate(flashlightStatsBase);
        }

        if (nightStickStatsBase != null)
        {
            nightStickStatsRuntime = Instantiate(nightStickStatsBase);
        }
        else
        {
            Debug.LogError("WeaponStatsManager: NightStickStatsBase atanmamış!");
        }
    }
}
