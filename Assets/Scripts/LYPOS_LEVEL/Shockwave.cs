using UnityEngine;
using DG.Tweening;

public class Shockwave : MonoBehaviour
{
    [Header("Combat Settings")]
    public float damage = 10f;
    public float duration = 2f;
    public float maxRadius = 5f; // Hedef Dünya Yarýçapý

    [Header("Jump Settings")]
    public float jumpHeightThreshold = 1.0f;

    [Header("Visual References")]
    public LineRenderer lineRenderer;
    public int segments = 50;

    private Transform player;
    private bool hasHitPlayer = false;
    private float currentWorldRadius = 0f;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        lineRenderer.positionCount = segments + 1;
        lineRenderer.useWorldSpace = false;

        Wave();
    }

    void Wave()
    {
        // 0'dan maxRadius'a kadar bir deðeri tween ediyoruz (Objeyi scale etmiyoruz!)
        DOTween.To(() => currentWorldRadius, x => currentWorldRadius = x, maxRadius, duration)
            .SetEase(Ease.OutQuad);

        lineRenderer.material.DOFade(0, duration).SetEase(Ease.InQuad);

        StartCoroutine(CheckWaveLogic());
    }

    System.Collections.IEnumerator CheckWaveLogic()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // Görseli çiz
            DrawCircle(currentWorldRadius);

            // Hasar kontrolü
            if (!hasHitPlayer && player != null)
            {
                float halfWidth = lineRenderer.startWidth / 2f;
                float outerVisualLimit = currentWorldRadius + halfWidth;
                float innerVisualLimit = currentWorldRadius - halfWidth;

                Vector3 bossPos = new Vector3(transform.position.x, 0, transform.position.z);
                Vector3 pPos = new Vector3(player.position.x, 0, player.position.z);
                float distToPlayer = Vector3.Distance(bossPos, pPos);

                if (distToPlayer < outerVisualLimit && distToPlayer > innerVisualLimit)
                {
                    float heightDiff = player.position.y - transform.position.y;
                    if (heightDiff < jumpHeightThreshold)
                    {
                        ApplyDamage();
                    }
                }
            }
            yield return null;
        }
        Destroy(gameObject);
    }

    void DrawCircle(float radius)
    {
        // ÖNEMLÝ: Eðer obje scale ediliyorsa, noktalarý scale deðerine bölerek nötrlüyoruz.
        // Eðer DOScale kullanmayý býraktýysak (yukarýdaki gibi) sadece radius yeterli.
        float divisionFactor = transform.localScale.x;
        if (divisionFactor == 0) divisionFactor = 1;

        float angle = 0f;
        for (int i = 0; i <= segments; i++)
        {
            float x = (Mathf.Sin(Mathf.Deg2Rad * angle) * radius) / divisionFactor;
            float z = (Mathf.Cos(Mathf.Deg2Rad * angle) * radius) / divisionFactor;
            lineRenderer.SetPosition(i, new Vector3(x, 0, z));
            angle += (360f / segments);
        }
    }

    void ApplyDamage()
    {
        PlayerHealthManager health = player.GetComponentInChildren<PlayerHealthManager>();
        if (health != null)
        {
            health.ModifyHealth(-damage);
            hasHitPlayer = true;
            Debug.Log("<color=red>Þok Dalgasý Ýsabeti!</color>");
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (lineRenderer == null) return;
        float r = Application.isPlaying ? currentWorldRadius : maxRadius;
        float halfWidth = lineRenderer.startWidth / 2f;

        UnityEditor.Handles.color = new Color(1, 0, 0, 0.5f);
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, r + halfWidth);
        UnityEditor.Handles.color = new Color(1, 1, 0, 0.5f);
        UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.up, r - halfWidth);
    }
#endif
}