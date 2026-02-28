using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    public enum CharacterType
    {
        WARRIOR,//jop male
        MAGE,//electro shock female
        RANGER//flashlight non selected
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

    public CharacterData[] characters = new CharacterData[3];

    [SerializeField] private GameObject player;
    private GameObject freeLookCamera;
    public EGameState gameState;

    [SerializeField] public Transform spawnObjectParent;

    public int currentHeroIndex;

    [SerializeField] private GameObject skillTreeUI;

    void Awake()
    {
        instance = this;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    void Start()
    {
        /*Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;*/
        SetGameState(EGameState.CHARACTERSELECTION);
        freeLookCamera = GameObject.Find("FreeLook Camera");

        ActiveControl(false);
        /*player = GameObject.Find("Third_Person_Player");
        player.GetComponent<ThirdPersonController>().enabled = false;
        ThirdPersonController.instance.playerInputActions.Player.Disable();*/

    }


    void Update()
    {
        //IT WILL MOVE TO UI MANAGER AND WILL ADD ON NEW INPUT SYSTEM
        /*if (Input.GetKeyDown(KeyCode.Tab) && !skillTreeUI.gameObject.activeSelf)
        {

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            ActiveControl(false);
            skillTreeUI.SetActive(!skillTreeUI.gameObject.activeSelf);

        }
        else if (skillTreeUI.gameObject.activeSelf && Input.GetKeyDown(KeyCode.Tab))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            ActiveControl(true);
            skillTreeUI.SetActive(!skillTreeUI.gameObject.activeSelf);
        }*/
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
    public void InGame(int selectedCharacterIndex)
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SetGameState(EGameState.INGAME);
        SpawnChrahterAtIndex(selectedCharacterIndex);
    }

    public void GameOverWin()
    {
        Time.timeScale = 0f;
        ActiveControl(false);
        SetGameState(EGameState.GAMEOVERWIN);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void GameOverLose()
    {
        Time.timeScale = 0f;
        ActiveControl(false);
        SetGameState(EGameState.GAMEOVERLOSE);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


    public void SpawnChrahterAtIndex(int index)
    {
        currentHeroIndex = index;
        Debug.Log(characters[index].characterName + " spawned");
        player = Instantiate(characters[index].characterPrefab, spawnObjectParent.position, spawnObjectParent.rotation, spawnObjectParent.transform);

        ThirdPersonController.instance.GetAnimatorCompononet();
        GameObject heroNameUI = GameObject.Find("HeroName");
        heroNameUI.GetComponent<TextMeshProUGUI>().text = characters[index].characterName;
        switch (index)
        {
            case 0://nighrstick cam
                ThirdPersonController.instance.currentCameraStyle = ThirdPersonController.CameraStyle.Combat;
                Debug.Log($"SpawnChrahterAtIndex() index: {index}: Kamera sistemi {ThirdPersonController.instance.currentCameraStyle} sistemine değiştri");
                break;
            case 1://beam cam
                ThirdPersonController.instance.currentCameraStyle = ThirdPersonController.CameraStyle.Shooter;
                Debug.Log($"SpawnChrahterAtIndex() index: {index}: Kamera sistemi {ThirdPersonController.instance.currentCameraStyle} sistemine değiştri");
                break;
        }
        ActiveControl(true);
        /*player.GetComponent<ThirdPersonController>().enabled = true;
        freeLookCamera.GetComponent<Cinemachine.CinemachineFreeLook>().enabled = true;
        ThirdPersonController.instance.playerInputActions.Player.Enable();*/

    }

    public void ActiveControl(bool isActive)
    {
        if (isActive)
        {
            freeLookCamera.GetComponent<Cinemachine.CinemachineFreeLook>().enabled = true;
            ThirdPersonController.instance.playerInputActions.Player.Enable();
        }
        else
        {
            freeLookCamera.GetComponent<Cinemachine.CinemachineFreeLook>().enabled = false;
            ThirdPersonController.instance.playerInputActions.Player.Disable();
        }
    }
    void OnDisable()
    {
        ThirdPersonController.instance.playerInputActions.Player.Disable();
    }

    void RemoveCurrentPlayer()
    {
        if (player != null)
        {
            Destroy(player);
        }
    }
}
