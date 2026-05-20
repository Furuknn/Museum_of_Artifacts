using System.Collections;
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

    private string _currentlyLoadedLevel;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) LoadLevel(level1SceneName);
        if (Input.GetKeyDown(KeyCode.Alpha2)) LoadLevel(level2SceneName);
        if (Input.GetKeyDown(KeyCode.Alpha3)) LoadLevel(level3SceneName);
        if (Input.GetKeyDown(KeyCode.Alpha4)) LoadLevel(level4SceneName);

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            StartCoroutine(UnloadScene());
            if(museumAssets!=null) museumAssets.SetActive(true);
            TeleportPlayerToMuseum();
        }
    }

    private void LoadLevel(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("[DebugLevelLoader] Scene name is empty.");
            return;
        }

        StartCoroutine(LoadRoutine(sceneName));
    }

    private IEnumerator LoadRoutine(string sceneName)
    {
        UnloadScene();

        AsyncOperation load = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!load.isDone) yield return null;

        _currentlyLoadedLevel = sceneName;

        Scene loaded = SceneManager.GetSceneByName(sceneName);
        if (loaded.IsValid())
            SceneManager.SetActiveScene(loaded);

        TeleportPlayerToSpawn();

        Debug.Log($"[DebugLevelLoader] Loaded: {sceneName}");

        if (museumAssets != null)
        {
            museumAssets.SetActive(false);
        }
    }

    private IEnumerator UnloadScene()
    {
        // Unload previous debug-loaded level if one is active
        if (!string.IsNullOrEmpty(_currentlyLoadedLevel))
        {
            Scene existing = SceneManager.GetSceneByName(_currentlyLoadedLevel);
            if (existing.IsValid() && existing.isLoaded)
            {
                yield return SceneManager.UnloadSceneAsync(_currentlyLoadedLevel);
                Debug.Log($"[DebugLevelLoader] Unloaded: {_currentlyLoadedLevel}");
            }
        }
    }

    private void TeleportPlayerToSpawn()
    {
        ThirdPersonController player = ThirdPersonController.Instance;
        if (player == null)
        {
            Debug.LogWarning("[DebugLevelLoader] ThirdPersonController.Instance is null.");
            return;
        }

        GameObject spawnPoint = GameObject.Find("PlayerSpawnPoint");
        if (spawnPoint == null)
        {
            Debug.LogWarning("[DebugLevelLoader] No PlayerSpawnPoint found in loaded scene.");
            return;
        }

        Vector3 targetPos = spawnPoint.transform.position + new Vector3(0f, spawnOffsetY, 0f);

        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
        player.transform.position = targetPos;
        if (cc != null) cc.enabled = true;
    }

    private void TeleportPlayerToMuseum()
    {
        ThirdPersonController player = ThirdPersonController.Instance;
        if (player == null)
        {
            Debug.LogWarning("[DebugLevelLoader] ThirdPersonController.Instance is null.");
            return;
        }

        GameObject spawnPoint = GameObject.Find("MuseumSpawnPoint");
        if (spawnPoint == null)
        {
            Debug.LogWarning("[DebugLevelLoader] No MuseumSpawnPoint found in loaded scene.");
            return;
        }

        Vector3 targetPos = spawnPoint.transform.position + new Vector3(0f, spawnOffsetY, 0f);

        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
        player.transform.position = targetPos;
        if (cc != null) cc.enabled = true;
    }
}
#endif