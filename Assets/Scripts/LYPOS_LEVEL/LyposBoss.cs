using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Drawing;

public enum BossState { Idle, Chasing, Attacking, Ability, Death }

public class LyposBoss : MonoBehaviour
{
    public BossState currentState;
    public Transform player;

    [Header("Prefabs")]
    public GameObject rock;
    public GameObject shockwave;
    [Header("References")]
    public GameObject parkour;
    public List<Transform> rockPos;
    public Transform rockParent;
    List<Transform> rocks = new List<Transform>();
    public Transform wand;
    public Transform wandCenter;
    public List<Transform> wandPoints;
    public List<Transform> chestParts = new List<Transform>();
    public Image healthImg;
    public GameObject healthBar;
    [Header("Stats")]
    public int wandStrikeDamage = 70;
    public int rockDamage = 70;
    public int impaleGroundDamage = 35;
    public float maxHealth = 3000;
    [Range(0f, 1f)] public float damageReduction = 0.5f;
    float _currentHealth;
    [Header("Settings")]
    public float attackCooldown = 3f;
    private float _nextAttackTime = 0f;
    public float rockLiftHeight = 7f;
    public float rockThrowInterval = 1f;
    public float exposeHeartTime = 15f;
    public float difficulty = 1f;
    bool _rocksReady = false;
    bool _usingSkill = false;
    bool _isHeartExposed = false;
    public bool _bossAwake = false;
    bool _isRotating = true;
    bool _bossPaused = false;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentState = BossState.Idle;
        _currentHealth = maxHealth;
    }

    public void StartBoss()
    {
        _bossAwake = true;
        healthBar.SetActive(true);
    }
    void Update()
    {
        if (!_bossAwake || _bossPaused) return;

        if (player == null) return;

        Deciding();

        HealthUI();
    }

    void HealthUI()
    {
        healthImg.fillAmount = _currentHealth / maxHealth;
    }
    void RotateTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // Boss'un yukarý aþaðý eðilmesini istemeyiz
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void Deciding()
    {
        if (_isRotating) RotateTowardsPlayer();

        if (Time.time >= _nextAttackTime)
        {
            DecideNextAttack();
        }
    }

    void DecideNextAttack()
    {
        if (_usingSkill) return;

        int randomAttack = Random.Range(0, 3); // 3 farklý saldýrý tipin olacak

        switch (randomAttack)
        {
            case 0:
                if (!_rocksReady) ImpaleGround();
                break;
            case 1:
                if (_rocksReady) LiftAndThrow();
                break;
            case 2:
                WandStrike();
                break;
        }

        // Bir sonraki saldýrý için bekleme süresini ayarla
        _nextAttackTime = Time.time + attackCooldown;
    }

    void ExposeHeart()
    {
        StartCoroutine(ExposeHeartRoutine());
    }

    public void TakeDamage(float damage)
    {
        if (!_isHeartExposed)
        {
            float finalDamage = damage * (1 - damageReduction);
            _currentHealth -= finalDamage;
        }
        else _currentHealth -= damage;

        if (_currentHealth < maxHealth / 2) difficulty = 2f;

        if (_currentHealth < 0) Death();
    }

    void Death()
    {
        healthBar.SetActive(false);
        Destroy(gameObject);
    }

    IEnumerator ExposeHeartRoutine()
    {
        parkour.SetActive(true);
        chestParts[0].DOLocalMoveZ(3.8f, 1f);
        chestParts[1].DOLocalMoveZ(-3.8f, 1f);

        yield return new WaitForSeconds(exposeHeartTime);

        chestParts[0].DOLocalMoveZ(2.3f, 1f);
        chestParts[1].DOLocalMoveZ(-2.3f, 1f);
        _usingSkill = false;
        parkour.SetActive(false);
    }

    void WandStrike()
    {
        _usingSkill = true;
        StartCoroutine(WandStrikeRoutine());
    }

    IEnumerator WandStrikeRoutine()
    {
        wand.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f / difficulty);
        wand.DOLocalRotate(new Vector3(90, 0, 0), 1.5f/difficulty).OnComplete(() => {
            
            foreach (var wandPoint in wandPoints)
            {
                Instantiate(shockwave, wandPoint.position, Quaternion.identity);

                Collider[] hitColliders = Physics.OverlapSphere(wandPoint.position, 0.75f, LayerMask.GetMask("Player"));

                foreach (var hitCollider in hitColliders)
                {
                    // Oyuncunun can sistemine eriþmeye çalýþ
                    PlayerHealthManager health = hitCollider.GetComponentInChildren<PlayerHealthManager>();

                    if (health != null)
                    {
                        health.ModifyHealth(-wandStrikeDamage);
                    }
                }
            }
            
        });
        yield return new WaitForSeconds(0.75f / difficulty);
        _isRotating = false;
        yield return new WaitForSeconds(2.25f/difficulty);
        wand.localRotation = Quaternion.identity;
        wand.DOLocalRotate(new Vector3(0, 0, 0), 2.5f / difficulty);
        //wand.gameObject.SetActive(false);
        _usingSkill = false;
        _isRotating = true;
    }
    void ImpaleGround()
    {
        _usingSkill = true;
        currentState = BossState.Attacking;

        wand.DOLocalMoveY(9, 1f).OnComplete(() => {
            wand.DOLocalMoveY(0, 0.2f).OnComplete(() => {
                ImpaleGroundEvent();
            }); ;
        }); ;
        
        
        // Burada animasyonu tetikle: animator.SetTrigger("Impale");

    }

    void ImpaleGroundEvent()
    {
        // Mevcut kayalarý temizle
        rocks.Clear();
        foreach (Transform child in rockParent)
        {
            if (child.childCount > 0 ) Destroy(child.GetChild(0).gameObject);
        }

        rockParent.position = new Vector3(rockParent.position.x, -2, rockParent.position.z);

        int rnd = Random.Range(5, 11);
        int r = 0;
        foreach (Transform t in rockPos)
        {
            GameObject newRock = Instantiate(rock, t.position, t.rotation);
            newRock.transform.SetParent(rockParent);
            rocks.Add(newRock.transform);
            r++;
            if (r == rnd) break;
        }

        rockParent.DOMoveY(2, 2).OnComplete(() => {
            currentState = BossState.Idle; // Saldýrý bitince Idle'a dön
            
        });

        Shockwave wave = Instantiate(shockwave, transform.position, transform.rotation).GetComponent<Shockwave>();
        wave.damage = impaleGroundDamage;
        wave.maxRadius = 50f;
        wave.duration = 4f;
        _rocksReady = true;
        _usingSkill = false;
    }

    void LiftAndThrow()
    {
        _usingSkill = true;
        StartCoroutine(LiftAndThrowRoutine());
    }

    Vector3 GetTacticalPosition(float radius)
    {
        // 1. Birim çember içinde rastgele bir 2D nokta seç (X ve Y düzleminde)
        Vector2 randomPoint = Random.insideUnitCircle * radius;

        // 2. Bu 2D noktayý 3D dünyaya uyarla (X ve Z eksenlerine yerleþtir)
        Vector3 offset = new Vector3(randomPoint.x, 0, randomPoint.y);

        // 3. Oyuncunun pozisyonuna bu offseti ekle
        Vector3 finalPosition = player.position + offset;

        return finalPosition;
    }

    IEnumerator LiftAndThrowRoutine()
    {
        ExposeHeart();
        wandCenter.DOLocalRotate(new Vector3(0, 0, 90), 2f/difficulty).OnComplete(() => {
            
        });
        rockParent.DOMoveY(rockLiftHeight, 2f/difficulty);
        yield return new WaitForSeconds(2.5f/difficulty);
        foreach (Transform rock in rocks)
        {
            wandCenter.DOLocalRotate(new Vector3(0, 0, 180), rockThrowInterval / 4).OnComplete(() => {
                wandCenter.DOLocalRotate(new Vector3(0, 0, 270), rockThrowInterval / 4).OnComplete(() => {
                    wandCenter.DOLocalRotate(new Vector3(0, 0, 360), rockThrowInterval / 4).OnComplete(() => {
                        wandCenter.rotation = Quaternion.identity;
                        wandCenter.DOLocalRotate(new Vector3(0, 0, 90), rockThrowInterval / 4);
                    }); ;
                });
            });
            rock.DOMove(GetTacticalPosition(3f), 0.75f).OnComplete(() =>
            {
                Instantiate(shockwave, rock.position, rock.rotation);

                Collider[] hitColliders = Physics.OverlapSphere(rock.position, 0.75f, LayerMask.GetMask("Player"));

                foreach (var hitCollider in hitColliders)
                {
                    // Oyuncunun can sistemine eriþmeye çalýþ
                    PlayerHealthManager health = hitCollider.GetComponentInChildren<PlayerHealthManager>();

                    if (health != null)
                    {
                        health.ModifyHealth(-rockDamage);
                    }
                }
            });
            yield return new WaitForSeconds(rockThrowInterval);
        }
        wandCenter.DOLocalRotate(new Vector3(0, 0, 0), 3f/difficulty);
        yield return new WaitForSeconds(2f/ difficulty);

        foreach (Transform rock in rocks)
        {
            Destroy(rock.gameObject);
        }
        rocks.Clear();
        _rocksReady = false;
    }

    private void OnEnable()
    {
        GameManager.OnGameStopped += OnGameStopped;
        GameManager.OnGameContinued += OnGameContinued;
    }

    private void OnDisable()
    {
        GameManager.OnGameStopped -= OnGameStopped;
        GameManager.OnGameContinued -= OnGameContinued;
    }

    private void OnGameStopped()
    {
        _bossPaused = true;
        //animator.speed = 0f;

    }

    private void OnGameContinued()
    {
        _bossPaused = false;
    }
}