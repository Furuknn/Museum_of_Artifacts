using System.Collections;
using UnityEngine;

public class MeshTrail : MonoBehaviour
{
    public float activeTime = 2f;

    [Header("Mesh Related")]
    public float meshRefreshRate = 0.1f;
    public float meshDestroyDelay = 3f;
    public Transform positionToSpawn;

    [Header("Shader Related")]
    public Material mat;
    public string shaderVarRef = "_Alpha";
    public float shaderVarRate = 0.1f;
    public float ShaderVarRefreshRate = 0.05f;

    private SkinnedMeshRenderer[] skinnedMeshRenderers;
    private bool isTrailActive;

    // ────────────────────────────────────────────────────────────────
    //  Public API — called by ThrenosController during the dash
    // ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Activate or deactivate the mesh trail externally.
    /// Passing <c>true</c> starts a trail that runs until you call
    /// <c>SetTrailActive(false)</c> or the coroutine naturally ends.
    /// </summary>
    public void SetTrailActive(bool active)
    {
        if (active && !isTrailActive)
        {
            isTrailActive = true;
            StartCoroutine(ActivateTrailIndefinite());
        }
        else if (!active)
        {
            // Setting this to false causes the running coroutine to exit
            // its while-loop on the next iteration
            isTrailActive = false;
        }
    }

    // ────────────────────────────────────────────────────────────────
    //  Original keyboard-triggered trail (kept for testing)
    // ────────────────────────────────────────────────────────────────

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            isTrailActive = true;
            StartCoroutine(ActivateTrail(activeTime));
        }
    }

    // ────────────────────────────────────────────────────────────────
    //  Original timed coroutine (unchanged)
    // ────────────────────────────────────────────────────────────────

    private IEnumerator ActivateTrail(float timeActive)
    {
        while (timeActive > 0)
        {
            timeActive -= meshRefreshRate;
            SpawnTrailMeshes();
            yield return new WaitForSeconds(meshRefreshRate);
        }
        isTrailActive = false;
    }

    // ────────────────────────────────────────────────────────────────
    //  New indefinite coroutine — runs until isTrailActive is false
    //  Used by ThrenosController.SetTrailActive(true/false)
    // ────────────────────────────────────────────────────────────────

    private IEnumerator ActivateTrailIndefinite()
    {
        while (isTrailActive)
        {
            SpawnTrailMeshes();
            yield return new WaitForSeconds(meshRefreshRate);
        }
    }

    // ────────────────────────────────────────────────────────────────
    //  Shared mesh baking & spawn logic (extracted to avoid duplication)
    // ────────────────────────────────────────────────────────────────

    private void SpawnTrailMeshes()
    {
        if (skinnedMeshRenderers == null)
            skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

        for (int i = 0; i < skinnedMeshRenderers.Length; i++)
        {
            GameObject obj = new GameObject();
            obj.transform.SetPositionAndRotation(positionToSpawn.position, positionToSpawn.rotation);

            MeshRenderer mr = obj.AddComponent<MeshRenderer>();
            MeshFilter mf = obj.AddComponent<MeshFilter>();

            Mesh mesh = new Mesh();
            skinnedMeshRenderers[i].BakeMesh(mesh);
            mf.mesh = mesh;

            Material[] trailMaterials = new Material[skinnedMeshRenderers[i].materials.Length];
            for (int j = 0; j < trailMaterials.Length; j++)
            {
                trailMaterials[j] = new Material(mat);
                StartCoroutine(AnimateMaterialFloat(trailMaterials[j], 0, shaderVarRate, ShaderVarRefreshRate));
            }

            mr.materials = trailMaterials;
            Destroy(obj, meshDestroyDelay);
        }
    }

    // ────────────────────────────────────────────────────────────────
    //  Material fade-out (unchanged, renamed for style consistency)
    // ────────────────────────────────────────────────────────────────

    private IEnumerator AnimateMaterialFloat(Material material, float goal, float rate, float refreshRate)
    {
        float value = material.GetFloat(shaderVarRef);
        while (value > goal)
        {
            value -= rate;
            material.SetFloat(shaderVarRef, value);
            yield return new WaitForSeconds(refreshRate);
        }
    }
}