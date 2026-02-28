using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class UIImageFall : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image image;

    [Header("Physics")]
    [SerializeField] private float gravityMultiplier = 3f;
    [SerializeField] private float torqueStrength = 10f;

    [Header("Fade")]
    [SerializeField] private float fadeSpeed = 1f;

    [Header("Input")]
    [SerializeField] private KeyCode triggerKey = KeyCode.Space;

    private Rigidbody rb;
    private bool isFalling = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (image == null)
            image = GetComponent<Image>();

        rb.isKinematic = true;
        rb.useGravity = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(triggerKey))
        {
            TriggerFall();
        }

        if (isFalling)
        {
            ApplyExtraGravity();
            FadeOut();
        }
    }

    public void TriggerFall()
    {
        if (isFalling) return;

        isFalling = true;

        rb.isKinematic = false;

        // Random torque
        Vector3 randomTorque = Random.insideUnitSphere * torqueStrength;
        rb.AddTorque(randomTorque, ForceMode.Impulse);
    }

    private void ApplyExtraGravity()
    {
        rb.AddForce(
            Physics.gravity * (gravityMultiplier - 1f),
            ForceMode.Acceleration
        );
    }

    private void FadeOut()
    {
        if (image == null) return;

        Color c = image.color;
        c.a = Mathf.MoveTowards(c.a, 0f, fadeSpeed * Time.deltaTime);
        image.color = c;
    }
}
