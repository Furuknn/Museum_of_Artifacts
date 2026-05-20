using UnityEngine;

public class SlowRotate : MonoBehaviour
{
    [SerializeField, Range(0f, 100f)] private float degreesPerSecond = 5f;

    void Update()
    {
        transform.Rotate(0f, degreesPerSecond * Time.deltaTime, 0f);
    }
}