using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
public class DebugLevelLoader : MonoBehaviour
{
    [Header("Key → Scene Name mapping")]
    [SerializeField] private string level1SceneName = "Level_01";
    [SerializeField] private string level2SceneName = "Level_02";
    [SerializeField] private string level3SceneName = "Level_03";
    [SerializeField] private string level4SceneName = "Level_04";

    [Tooltip("Hide the Museum when loading scenes")]
    [SerializeField] private GameObject museumAssets;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnOffsetY = 3f;

    // Fast lookup hash set filled automatically on Awake/Validate
    private HashSet<string> _allManagedLevels = new HashSet<string>();

    private void Awake()
    {
        InitializeLevelLookups();
    }

    private void OnValidate()
    {
        InitializeLevelLookups();
    }

    private void InitializeLevelLookups()
    {
        _allManagedLevels.Clear();
        if (!string.IsNullOrEmpty(level1SceneName)) _allManagedLevels.Add(level1SceneName);
        if (!string.IsNullOrEmpty(level2SceneName)) _allManagedLevels.Add(level2SceneName);
        if (!string.IsNullOrEmpty(level3SceneName)) _allManagedLevels.Add(level3SceneName);
        if (!string.IsNullOrEmpty(level4SceneName)) _allManagedLevels.Add(level4SceneName);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) LoadLevel(level1SceneName);
        if (Input.GetKeyDown(KeyCode.Alpha2)) LoadLevel(level2SceneName);
        if (Input.GetKeyDown(KeyCode.Alpha3)) LoadLevel(level3SceneName);
        if (Input.GetKeyDown(KeyCode.Alpha4)) LoadLevel(level4SceneName);

        if (Input.GetKeyDown(KeyCode.Alpha0))
            StartCoroutine(ReturnToMuseum());
    }

    private void LoadLevel(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("[DebugLevelLoader] Scene name is empty.");
            return;
        }

        if (sceneName == "dolvarisScene") GameManager.Instance.SetUIAndCamera(false);
        else GameManager.Instance.SetUIAndCamera(true);

        StartCoroutine(LoadRoutine(sceneName));
    }

    private IEnumerator LoadRoutine(string sceneName)
    {
        // 1. Scrub away any currently open managed level first
        yield return StartCoroutine(UnloadAnyOpenManagedLevels());

        // 2. Additively load the new targeted scene
        AsyncOperation load = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!load.isDone) yield return null;

        Scene loaded = SceneManager.GetSceneByName(sceneName);
        if (loaded.IsValid())
            SceneManager.SetActiveScene(loaded);

        TeleportPlayerToSpawn();
        if (museumAssets != null) museumAssets.SetActive(false);

        Debug.Log($"[DebugLevelLoader] Cleanly Loaded: {sceneName}");
    }

    private IEnumerator ReturnToMuseum()
    {
        yield return StartCoroutine(UnloadAnyOpenManagedLevels());
        if (museumAssets != null) museumAssets.SetActive(true);
        GameManager.Instance.SetUIAndCamera(true);
        TeleportPlayerToMuseum();
    }

    private IEnumerator UnloadAnyOpenManagedLevels()
    {
        // Iterate backwards through all open scenes in Unity's hierarchy manager
        for (int i = SceneManager.sceneCount - 1; i >= 0; i--)
        {
            Scene scene = SceneManager.GetSceneAt(i);

            // If this running scene matches one of your level inspector strings, clean it up!
            if (scene.IsValid() && scene.isLoaded && _allManagedLevels.Contains(scene.name))
            {
                Debug.Log($"[DebugLevelLoader] Deep Clean: Unloading active scene '{scene.name}'");
                yield return SceneManager.UnloadSceneAsync(scene);
            }
        }
    }

    // --- Teleportation logic remains unchanged ---
    private void TeleportPlayerToSpawn()
    {
        ThirdPersonController player = ThirdPersonController.Instance;
        if (player == null) return;

        GameObject spawnPoint = GameObject.Find("PlayerSpawnPoint");
        if (spawnPoint == null) return;

        Vector3 targetPos = spawnPoint.transform.position + new Vector3(0f, spawnOffsetY, 0f);
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
        player.transform.position = targetPos;
        if (cc != null) cc.enabled = true;
    }

    private void TeleportPlayerToMuseum()
    {
        ThirdPersonController player = ThirdPersonController.Instance;
        if (player == null) return;

        GameObject spawnPoint = GameObject.Find("MuseumSpawnPoint");
        if (spawnPoint == null) return;

        Vector3 targetPos = spawnPoint.transform.position + new Vector3(0f, spawnOffsetY, 0f);
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
        player.transform.position = targetPos;
        if (cc != null) cc.enabled = true;
    }
}
#endif