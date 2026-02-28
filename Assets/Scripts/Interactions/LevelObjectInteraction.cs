using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelObjectInteraction : MonoBehaviour, IInteractable
{

    [SerializeField] private string loadToScene;
    [SerializeField] private float tpPlayerPosY = 3;
    private MeshRenderer meshRenderer;
    private Material originalMaterial;
    private Material cursedMaterial;


    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            meshRenderer = GetComponentInChildren<MeshRenderer>();
        }
    }
    public void Interact()
    {
        LevelManager.instance.SaveLastInteractArtifact(this.gameObject.GetComponent<LevelObjectInteraction>());
        LevelManager.instance.ModifyFormerPositionForReturn(this.transform.position + (this.transform.forward * 3));
        Debug.Log($"Obje ile etkileşime geçildi {loadToScene} yükleniyor");

        StartCoroutine(LoadSceneAndTeleport());
        /*SceneManager.LoadScene(loadToScene, LoadSceneMode.Additive);

        playerSpawnPoint.transform.position = transform.Find("PlayerSpawnPoint").position + new Vector3(0, tpPlayerPosY, 0);
        ThirdPersonController.instance.characterController.Move(playerSpawnPoint.transform.position);*/
    }

    IEnumerator LoadSceneAndTeleport()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(loadToScene, LoadSceneMode.Additive);
        LevelManager.instance.ModifyCurrentLevelName(loadToScene);
        while (!asyncOperation.isDone)
        {
            yield return null;
        }

        TeleportPlayerToLevel();


        //SetDefaultMaterial();
    }
    private void TeleportPlayerToLevel()
    {
        var player = ThirdPersonController.instance;
        GameObject spawnObj = GameObject.Find("PlayerSpawnPoint");
        Vector3 targetPos = spawnObj.transform.position + new Vector3(0, tpPlayerPosY, 0);

        LevelManager.instance.ActiveMuseum(false);
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
        }

        player.transform.position = targetPos;

        if (cc != null)
        {
            cc.enabled = true;
        }
    }

    public void TeleportPlayerBack()
    {

    }

    public void ChangeMaterialOfArtifact(Material brokenMaterial)
    {
        if (meshRenderer == null)
        {
            meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer == null) meshRenderer = GetComponentInChildren<MeshRenderer>();
        }
        cursedMaterial = brokenMaterial;
        if (meshRenderer != null)
        {
            if (originalMaterial == null)
            {
                originalMaterial = meshRenderer.material;
            }
            Debug.Log($"{gameObject.name} lanetleniyor (Material ekleniyor)...");
            /*List<Material> materialList = new List<Material>(meshRenderer.materials);
            materialList.Add(brokenMaterial);
            meshRenderer.materials = materialList.ToArray();*/
            meshRenderer.material = cursedMaterial;
        }
        else
        {
            Debug.LogError($"HATA: {gameObject.name} objesinde MeshRenderer bulunamadı! Material değiştirilemiyor.");
        }
    }
    public void SetDefaultMaterial()
    {
        /*if (meshRenderer != null)
        {
            List<Material> materialList = new List<Material>(meshRenderer.materials);
            bool materialFound = false;
            for (int i = materialList.Count - 1; i >= 0; i--)
            {
                if (materialList[i].name.StartsWith(cursedMaterial.name))
                {
                    materialList.RemoveAt(i);
                    break;
                }
            }

            if (materialFound)
            {
                meshRenderer.materials = materialList.ToArray();
            }

        }
        else
        {
            Debug.Log("Null");
        }*/
        if (meshRenderer != null && originalMaterial != null)
        {
            // Değişiklik 3: Yedeklediğimiz orijinal materyali geri yüklüyoruz.
            meshRenderer.material = originalMaterial;
            Debug.Log("Materyal normale döndü.");
        }
        else
        {
            Debug.LogWarning("Orijinal materyal bulunamadı veya renderer yok.");
        }
    }

    public void AppendLevelName(string levelName)
    {
        loadToScene = levelName;
    }
}
