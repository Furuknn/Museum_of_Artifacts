using System.Collections;
using UnityEngine;

public class EnemyHitFeedback : MonoBehaviour
{
    private SkinnedMeshRenderer skinnedMesh;
    private MaterialPropertyBlock mpb;

    
    [SerializeField]private float decaySpeed = 6f;
    [SerializeField] private float maxScale = 0.02f;
    private float scaleValue = 0f;

    private static readonly int ScaleID = Shader.PropertyToID("_Scale");

    void Awake()
    {
        skinnedMesh = GetComponentInChildren<SkinnedMeshRenderer>();
        mpb = new MaterialPropertyBlock();
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
        scaleValue = maxScale; // spike
        ApplyScale(scaleValue);
    }

    private void ApplyScale(float value)
    {
        // IMPORTANT: material index 1
        skinnedMesh.GetPropertyBlock(mpb, 1);
        mpb.SetFloat(ScaleID, value);
        skinnedMesh.SetPropertyBlock(mpb, 1);
    }
}
