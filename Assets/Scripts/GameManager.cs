using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public enum CharacterType
    {
        NIGHTSTICK,
        FLASHLIGHT,
        TASER
    }
    [System.Serializable]
    public class CharacterData
    {
        public CharacterType characterType;
        public string characterName;
        public GameObject characterPrefab;
        //public Sprite weaponIcon;
        public PlayerStatisticsSO playerStatisticsSO;
    }

    public static System.Action OnGameStopped;
    public static System.Action OnGameContinued;

    public CharacterData[] characters = new CharacterData[3];

    public GameObject PlayerParentObject;
    [SerializeField] private GameObject player;
    private GameObject freeLookCamera;
    public EGameState gameState;

    [SerializeField] public Transform spawnObjectParent;

    public int currentHeroIndex;

    public CinemachineFreeLook freeLook;
    private GameObject ui;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Instance = this;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    void Start()
    {
        SetGameState(gameState);
        freeLookCamera = GameObject.Find("FreeLook Camera");

        ActiveControl(false);

        freeLook = FindAnyObjectByType<CinemachineFreeLook>();
        ui = GameObject.Find("UI");
    }

    void OnDestroy()
    {
        SetGameState(gameState);
        Debug.Log("GameManager imha ediliyor! İz sürülüyor...", this);
        Debug.Log(System.Environment.StackTrace);
    }

    void Update()
    {

    }

    public void RestartDolvaris()
    {
        StartCoroutine(RestartRoutine());
    }

    public IEnumerator RestartRoutine()
    {
        AsyncOperation load = SceneManager.UnloadSceneAsync("dolvarisScene");
        while (!load.isDone) yield return null;

        AsyncOperation reload = SceneManager.LoadSceneAsync("dolvarisScene", LoadSceneMode.Additive);
        while (!reload.isDone) yield return null;

        SceneManager.SetActiveScene(SceneManager.GetSceneByName("dolvarisScene"));
    }

    public void SetGameState(EGameState gameState)
    {
        this.gameState = gameState;
        IEnumerable<IGameStateListener> gameStateListeners = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IGameStateListener>();

        foreach (IGameStateListener dependency in gameStateListeners)
        {
            dependency.GameStateChangedCallBack(gameState);
        }
    }

    public void GameIntro()
    {
        SetGameState(EGameState.GAMEINTRO);
    }

    public void ToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void CharacterSelection()
    {
        SetGameState(EGameState.CHARACTERSELECTION);
    }
    public void InGame()
    {
        ContinueGame();
        SpawnCharacterAtIndex(currentHeroIndex);
    }

    public void GameOverWin()
    {
        StopGame(EGameState.GAMEOVERWIN);
    }
    public void GameOverLose()
    {
        StopGame(EGameState.GAMEOVERLOSE);
    }

    public void StopGame(EGameState gameState)
    {
        ActiveControl(false);
        SetGameState(gameState);
        Cursor.lockState= CursorLockMode.None;
        Cursor.visible = true;

        OnGameStopped?.Invoke();
    }

    public void ContinueGame()
    {
        ActiveControl(true);
        SetGameState(EGameState.INGAME);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        OnGameContinued?.Invoke();
    }


    public void SpawnCharacterAtIndex(int index)
    {
        currentHeroIndex = index;
        Debug.Log(characters[index].characterName + " spawned");
        player = Instantiate(characters[index].characterPrefab, spawnObjectParent.position, spawnObjectParent.rotation, spawnObjectParent.transform);

        ThirdPersonController.Instance.GetAnimatorComponent();
        GameObject heroNameUI = GameObject.Find("HeroName");
        heroNameUI.GetComponent<TextMeshProUGUI>().text = characters[index].characterName;
        switch (index)
        {
            case 0://nighrstick cam
                ThirdPersonController.Instance.currentCameraStyle = ThirdPersonController.CameraStyle.Combat;
                Debug.Log($"SpawnCharacterAtIndex() index: {index}: Kamera sistemi {ThirdPersonController.Instance.currentCameraStyle} sistemine değişti");
                break;
            case 1://beam cam
                ThirdPersonController.Instance.currentCameraStyle = ThirdPersonController.CameraStyle.Shooter;
                Debug.Log($"SpawnCharacterAtIndex() index: {index}: Kamera sistemi {ThirdPersonController.Instance.currentCameraStyle} sistemine değişti");
                break;
        }
        ActiveControl(true);

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

    public void ActiveControl(bool isActive)
    {
        if (isActive)
            freeLookCamera.GetComponent<Cinemachine.CinemachineFreeLook>().enabled = true;
        else
            freeLookCamera.GetComponent<Cinemachine.CinemachineFreeLook>().enabled = false;
    }

    public void SetUIAndCamera(bool set)
    {
        
        if (freeLook != null) freeLook.gameObject.SetActive(set);

        if (ui != null) ui.SetActive(set);

        if (set == false)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        GameManager.Instance.PlayerParentObject.SetActive(set);
    }
    void OnDisable()
    {
        ThirdPersonController.Instance.playerInputActions.Player.Disable();
    }

    public void SetHeroIndex(int index)
    {
        currentHeroIndex = index;
    }
}
