using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class enemyDealedDamageUI : MonoBehaviour
{
    [Header("UI Bileşenleri")]
    [SerializeField] private TextMeshProUGUI damageText;

    [Header("Animasyon Ayarları")]
    [SerializeField] private float moveSpeed = 1f; // Yukarı hareket hızı
    [SerializeField] private float fadeDuration = 1.0f; // Kaybolma süresi
    [SerializeField] private float lifeTime = 2.0f; // Toplam yaşam süresi
    [SerializeField] private float scaleDuration = 0.2f; // Scale animasyonu süresi

    [Header("Renk Ayarları")]
    [SerializeField] private Color smallDamageColor = Color.white;
    [SerializeField] private Color largeDamageColor = Color.red;

    private float targetScale;
    public void Initialize(float damage, float finalScale)
    {
        // Değerleri ata
        damageText.text = Mathf.RoundToInt(damage).ToString();
        targetScale = finalScale;

        float normalizedColorRatio = (finalScale - 1.0f) / 1.0f;
        damageText.color = Color.Lerp(smallDamageColor, largeDamageColor, normalizedColorRatio);

        // Animasyonları başlat
        StartCoroutine(AnimateDamageText());
    }

    private void Start()
    {
        if (Camera.main != null)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        }
    }

    private IEnumerator AnimateDamageText()
    {
        Vector3 initialScale = Vector3.one * 0.1f;
        Vector3 finalScaleVector = Vector3.one * targetScale;
        float timeElapsed = 0f;

        while (timeElapsed < scaleDuration)
        {
            transform.localScale = Vector3.Lerp(initialScale, finalScaleVector, timeElapsed / scaleDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        transform.localScale = finalScaleVector;

        float moveAndFadeTime = lifeTime - scaleDuration;
        float elapsedTime = 0f;
        Color startColor = damageText.color;

        while (elapsedTime < moveAndFadeTime)
        {
            transform.position += Vector3.up * moveSpeed * Time.deltaTime;

            if (elapsedTime > (moveAndFadeTime - fadeDuration))
            {
                float fadeRatio = (elapsedTime - (moveAndFadeTime - fadeDuration)) / fadeDuration;
                Color newColor = Color.Lerp(startColor, new Color(startColor.r, startColor.g, startColor.b, 0), fadeRatio);
                damageText.color = newColor;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
