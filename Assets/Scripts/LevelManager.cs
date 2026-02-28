using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    private string currentMainLevel;
    [Header("Tree Settings")]
    [SerializeField] private GameObject treeGO;
    [SerializeField] private Transform treeSpawnTransform;
    private GameObject tree;
    [Header("Museum Artifacts")]
    private bool isMuseumArtifactsCursed = false;
    [System.Serializable]
    public class LevelData
    {
        public string name;
        public GameObject artifactParent; // old: museumArtifacts
        public string levelSceneName;     // old: museumArtifactsLevelName
        public Material specificCursedMaterial;
    }
    [Header("Level Configurations")]
    [SerializeField] private List<LevelData> levels;
    [SerializeField] private Material defaultCursedMaterial;
    [Header("Game Mechanic")]
    private Vector3 savedPlayerReturnPosition;
    [SerializeField] public bool isPlayerGetFirstWin = false;
    [SerializeField] private string currentLevelName;
    private LevelObjectInteraction levelObjectInteraction;
    private int levelCount;
    private int totalWinCount;
    [Header("Museum")]
    [SerializeField] private GameObject museum;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        currentMainLevel = this.gameObject.scene.name;
        Debug.Log($"Level manager'a ait Suan ki levelin adi: {currentMainLevel}");
        levelCount = levels.Count;
        Debug.Log($"Toplam level sayisi: {levelCount}");
    }
    public void InstantiateTree()
    {
        if (tree != null) return;

        //Debug.Log("InstantiateTree(): Agac olusturuluyor");
        tree = Instantiate(treeGO, treeSpawnTransform);
    }
    public void ArtifectsInMuseum()
    {
        isMuseumArtifactsCursed = true;
        Debug.LogWarning("eserlerde bir gariplik var");
        foreach (LevelData currentLevel in levels)
        {
            GameObject parentGO = currentLevel.artifactParent;
            if (parentGO == null) continue;
            Debug.Log(parentGO.transform.childCount);

            int childCount = parentGO.transform.childCount;
            int selectedArtifact = Random.Range(0, childCount);
            //Debug.Log($" {parents.name} iteminin secilen {parents.transform.GetChild(selectedArtifact).name}");
            Transform selectedChild = parentGO.transform.GetChild(selectedArtifact);

            Debug.Log($"Random result: {selectedArtifact} name: {selectedChild.name}");
            LevelObjectInteraction interaction = selectedChild.AddComponent<LevelObjectInteraction>();
            interaction.ChangeMaterialOfArtifact(defaultCursedMaterial);

            interaction.AppendLevelName(currentLevel.levelSceneName);//SWITCH CASE WAS REMOVED

        }
    }
    public void BackToTheFormerPosition()
    {
        ModifyFormerPositionForReturn(levelObjectInteraction.transform.position + (levelObjectInteraction.transform.forward * 3));
    }
    public void ModifyFormerPositionForReturn(Vector3 playersPosition)
    {
        savedPlayerReturnPosition = playersPosition;
        Debug.Log($"Player's former postion saved: {savedPlayerReturnPosition}");

    }

    public void ReturnFromLevel()
    {
        ActiveMuseum(true);
        var player = ThirdPersonController.instance;
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
        }

        player.transform.position = savedPlayerReturnPosition;

        if (cc != null)
        {
            cc.enabled = true;
        }
    }
    public void ModifyCurrentLevelName(string loadToScene)
    {
        currentLevelName = loadToScene;
    }

    public void DestroyCurrentLevel()
    {
        SceneManager.UnloadSceneAsync(currentLevelName);
    }

    public bool isMuseumArtifactsCursedCheck()
    {
        return isMuseumArtifactsCursed;
    }

    public void ResetCurrentMainLevel()
    {
        if (string.IsNullOrEmpty(currentMainLevel))
        {
            Debug.LogError("CurrentMainLevel ismi boş! Reset atılamıyor.");
            return;
        }

        Debug.Log($"{currentMainLevel} sahnesi baştan başlatılıyor (Reset)...");

        SceneManager.LoadScene(currentMainLevel, LoadSceneMode.Single);
    }

    public void SaveLastInteractArtifact(LevelObjectInteraction artifactScript)
    {
        levelObjectInteraction = artifactScript;
    }
    public void ReturnWithWinFromLevel()
    {

        Debug.Log("Win");
        totalWinCount++;
        isPlayerGetFirstWin = true;

        //ModifyFormerPositionForReturn(levelObjectInteraction.transform.position + (levelObjectInteraction.transform.forward * 3));
        levelObjectInteraction.SetDefaultMaterial();
        Destroy(levelObjectInteraction.gameObject.GetComponent<LevelObjectInteraction>());

        if (totalWinCount == levelCount)
        {
            GameManager.instance.GameOverWin();
        }

    }
    public void ReturnWithLoseFromLevel()
    {
        if (!isPlayerGetFirstWin)
        {
            //COMPLETLY LOSE SCREEN
            Debug.Log("COMPLETLY Lose Screen");
            GameManager.instance.GameOverLose();
            //ResetCurrentMainLevel();
        }
        else
        {
            //Respawn player to museum with lose
            Debug.Log("CLASSIC Lose Screen");
            BackToTheFormerPosition();
        }
    }
    public void ActiveMuseum(bool active)
    {
        museum.SetActive(active);
    }

}
#if UNITY_EDITOR


[CustomEditor(typeof(LevelManager))]
public class LevelManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelManager script = (LevelManager)target;

        GUILayout.Space(10);

        GUI.enabled = Application.isPlaying;



        if (GUILayout.Button("Create Tree"))
        {
            script.InstantiateTree();
        }
    }
}
#endif
