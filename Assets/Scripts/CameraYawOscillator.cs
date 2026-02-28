using UnityEngine;

public class CameraYawOscillator : MonoBehaviour
{
    [SerializeField] private float maxRotateAngle = 30f; // degrees
    [SerializeField] private float rotationSpeed = 30f;  // degrees per second

    private float currentAngle = 0f;
    private int direction = 1; // 1 = right, -1 = left

    private void Update()
    {
        currentAngle += direction * rotationSpeed * Time.deltaTime;

        if (Mathf.Abs(currentAngle) >= maxRotateAngle)
        {
            currentAngle = Mathf.Clamp(currentAngle, -maxRotateAngle, maxRotateAngle);
            direction *= -1;
        }

        Vector3 euler = transform.localEulerAngles;
        euler.y = currentAngle;
        transform.localEulerAngles = euler;
    }
}
