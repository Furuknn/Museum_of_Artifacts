using System.Collections;
using UnityEngine;

public class EnemyHitFeedback : MonoBehaviour
{
    private SkinnedMeshRenderer skinnedMesh;
    private MaterialPropertyBlock mpb;

    [Header("Material Settings")]
    [Tooltip("The specific material to apply the hit feedback to.")]
    [SerializeField] private Material targetMaterial;
    private int targetMaterialIndex = -1; // -1 means not found yet

    [Header("Feedback Settings")]
    [SerializeField] private float decaySpeed = 6f;
    [SerializeField] private float maxScale = 0.02f;
    private float scaleValue = 0f;

    private static readonly int ScaleID = Shader.PropertyToID("_Scale");

    void Awake()
    {
        skinnedMesh = GetComponentInChildren<SkinnedMeshRenderer>();
        mpb = new MaterialPropertyBlock();

        FindTargetMaterialIndex();
    }

    void Update()
    {
        if (scaleValue > 0f)
        {
            scaleValue = Mathf.MoveTowards(scaleValue, 0f, decaySpeed * Time.deltaTime);
            ApplyScale(scaleValue);
        }
    }

    public void OnHit()
    {
        // Don't bother if we didn't find the material
        if (targetMaterialIndex == -1) return;

        scaleValue = maxScale; // spike
        ApplyScale(scaleValue);
    }

    private void ApplyScale(float value)
    {
        if (targetMaterialIndex == -1) return;

        skinnedMesh.GetPropertyBlock(mpb, targetMaterialIndex);
        mpb.SetFloat(ScaleID, value);
        skinnedMesh.SetPropertyBlock(mpb, targetMaterialIndex);
    }

    private void FindTargetMaterialIndex()
    {
        if (skinnedMesh == null || targetMaterial == null) return;

        // Iterate through sharedMaterials to find the matching reference
        Material[] mats = skinnedMesh.sharedMaterials;
        for (int i = 0; i < mats.Length; i++)
        {
            if (mats[i] == targetMaterial)
            {
                targetMaterialIndex = i;
                return; // Found it, stop searching
            }
        }

        // If we get here, the material wasn't on the mesh
        Debug.LogWarning($"[EnemyHitFeedback] The material '{targetMaterial.name}' was not found on {skinnedMesh.name}'s SkinnedMeshRenderer!");
    }
}