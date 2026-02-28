using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XpGainObject : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] private float forceMultiply = 2f;
    public float xpGain;

    private GameObject playerGO;

    [Header("Detection Settings")]
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float pickupRange = 0.5f;

    [Header("Movement Settings")]
    [SerializeField] private float minSpeed = 5f;
    [SerializeField] private float accelerationFactor = 10f;

    // Durum kontrolü
    private bool isActive = false;
    private bool isSpawnAnimationFinished = false;
    [SerializeField] private float delayBeforeCheck = 0.5f;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        //if(GetComponent<Collider>()) GetComponent<Collider>().isTrigger = true; 

        playerGO = FindObjectOfType<ThirdPersonController>().gameObject;

        transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        rb.AddForce(transform.forward * forceMultiply + Vector3.up * forceMultiply, ForceMode.Impulse);

        StartCoroutine(SpawnDelayRoutine());
    }

    private void Update()
    {

        if (playerGO == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerGO.transform.position);
        //Debug.Log(distanceToPlayer);
        if (!isActive && isSpawnAnimationFinished)
        {
            if (distanceToPlayer <= detectionRange)
            {
                ActivateMovement();
            }
        }

        if (isActive)
        {
            MoveToPlayer(distanceToPlayer);
            CheckPickup(distanceToPlayer);
        }
    }

    IEnumerator SpawnDelayRoutine()
    {
        yield return new WaitForSeconds(delayBeforeCheck);
        isSpawnAnimationFinished = true;
    }

    private void ActivateMovement()
    {
        isActive = true;
        //rb.isKinematic = true;
        //rb.useGravity = false;
    }

    private void MoveToPlayer(float currentDistance)
    {
        float currentSpeed = minSpeed + (accelerationFactor / (currentDistance + 0.1f));

        Vector3 targetPos = playerGO.transform.position + Vector3.up * 1.0f;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, currentSpeed * Time.deltaTime);
    }

    private void CheckPickup(float currentDistance)
    {
        if (currentDistance <= pickupRange)
        {
            CollectXp();
        }
    }

    private void CollectXp()
    {
        PlayerXpManagement.instance.ModifyXp(xpGain);

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}