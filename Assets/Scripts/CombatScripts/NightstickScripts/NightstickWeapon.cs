using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG;
using DG.Tweening;

public class NightstickWeapon : WeaponBase
{
    float _mainAttackDamage => WeaponStatsManager.Instance.nightStickStatsRuntime.mainAttackDamage * WeaponStatsManager.Instance.nightStickStatsRuntime.damageMultiply;
    float _attackSpeed => WeaponStatsManager.Instance.nightStickStatsRuntime.attackSpeed;
    float _attackRange => WeaponStatsManager.Instance.nightStickStatsRuntime.attackRange;

    float _smashGroundDamage => WeaponStatsManager.Instance.nightStickStatsRuntime.smashGroundDamage;
    float _smashGroundRadius => WeaponStatsManager.Instance.nightStickStatsRuntime.smashGroundRadius;
    float _smashGroundStunTime => WeaponStatsManager.Instance.nightStickStatsRuntime.smashGroundStunTime;
    float _smashGroundCooldown => WeaponStatsManager.Instance.nightStickStatsRuntime.smashGroundCooldown;
    bool _smashActive => WeaponStatsManager.Instance.nightStickStatsRuntime.isSmashActive;

    float _spinDamage => WeaponStatsManager.Instance.nightStickStatsRuntime.spinDamage;
    float _spinRadius => WeaponStatsManager.Instance.nightStickStatsRuntime.spinRadius;
    float _spinSpeed => WeaponStatsManager.Instance.nightStickStatsRuntime.spinSpeed;
    float _spinPlayerSpeed => WeaponStatsManager.Instance.nightStickStatsRuntime.spinPlayerSpeed;
    float _spinDuration => WeaponStatsManager.Instance.nightStickStatsRuntime.spinDuration;
    float _spinCooldown => WeaponStatsManager.Instance.nightStickStatsRuntime.spinCooldown;
    bool _spinHealthRegen => WeaponStatsManager.Instance.nightStickStatsRuntime.spinHealthRegen;
    bool _spinActive => WeaponStatsManager.Instance.nightStickStatsRuntime.isSpinActive;

    float _dashDamage => WeaponStatsManager.Instance.nightStickStatsRuntime.dashDamage;
    float _dashRange => WeaponStatsManager.Instance.nightStickStatsRuntime.dashRange;
    float _dashWidth => WeaponStatsManager.Instance.nightStickStatsRuntime.dashWidth;
    float _dashCooldown => WeaponStatsManager.Instance.nightStickStatsRuntime.dashCooldown;
    bool _dashImmunity => WeaponStatsManager.Instance.nightStickStatsRuntime.dashImmunity;
    bool _dashActive => WeaponStatsManager.Instance.nightStickStatsRuntime.isDashActive;
    

    float _shieldDuration => WeaponStatsManager.Instance.nightStickStatsRuntime.shieldDuration;
    bool _shieldSlowness => WeaponStatsManager.Instance.nightStickStatsRuntime.shieldSlowness;
    bool _deflectDamage => WeaponStatsManager.Instance.nightStickStatsRuntime.damageDeflect;
    float _shieldCooldown => WeaponStatsManager.Instance.nightStickStatsRuntime.shieldCooldown;

    bool _canAttack = true;
    bool _canSmash = true;
    bool _canSpin = true;
    bool _canDash = true;
    bool _canUlti = true;
    bool _isDashing = false;

    public LayerMask attackLayer;
    public Animator anim;
    public MeshRenderer smashGroundCircle;
    public GameObject smashEffect;
    public ParticleSystem smashGroundParticle;
    public BoxCollider dashCollider;
    public ParticleSystem dashParticle;
    public GameObject shield;
    PlayerHealthManager health;
    CharacterController controller;
    Vector3 dashStartPos;
    Vector3 forward;

    private void Start()
    {
        health = PlayerHealthManager.Instance;
        controller = ThirdPersonController.Instance.gameObject.GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) UltimateAttack();
    }
    public override void MainAttack()
    {
        if (_canAttack)
        {
            RotateToCam();
            anim.SetTrigger("Attack");
            _canAttack = false;
        }
        
    }

    private void OnEnable()
    {
        GameManager.OnGameContinued += OnGameContinued;
        GameManager.OnGameStopped += OnGameStopped;
    }

    private void OnDisable()
    {
        GameManager.OnGameContinued -= OnGameContinued;
        GameManager.OnGameStopped -= OnGameStopped;
    }

    private void OnGameStopped()
    {
        _canAttack = false;
        _canSmash = false;
        _canSpin = false;
        _canDash = false;
        _canUlti = false;
    }
    private void OnGameContinued()
    {
        _canAttack = true;
        _canSmash = true;
        _canSpin = true;
        _canDash = true;
        _canUlti = true;
    }

    void RotateToCam()
    {
        Transform player = ThirdPersonController.Instance.transform;

        forward = Camera.main.transform.forward;
        forward.y = 0;
        forward.Normalize();

        Quaternion targetRotation = Quaternion.LookRotation(forward);

        // DOTween ile bu rotasyona yumuþak geçiþ yapýyoruz
        player.DORotate(targetRotation.eulerAngles, 0.5f);
    }
    public void MainAttackEvent()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        if (Physics.Raycast(ray, out hit, _attackRange, attackLayer))
        {
            EnemyScript enemy = hit.collider.gameObject.GetComponent<EnemyScript>();
            if (enemy != null)
            {
                enemy.TakeDamage(_mainAttackDamage);
            }
        }
        HUDManager.hudManager.StartCooldown(0.6f, 0);
    }

    public void ResetMainAttackEvent()
    {
        ToggleHandLayer(true);
        _canAttack = true;
        anim.SetTrigger("Reset");
        ThirdPersonController.Instance.canJump = true;
    }

    public override void SecondaryAttack()
    {
        if (_smashActive && _canSmash)
        {
            ThirdPersonController.Instance.canJump = false;
            _canAttack = false;
            _canSmash = false;
            ToggleHandLayer(false);
            anim.SetTrigger("SmashAttack");
            StartCoroutine(M2SmashCooldown());
        }
        else if (_spinActive && _canSpin)
        {
            ThirdPersonController.Instance.canJump = false;
            StartCoroutine(SpinRoutine());
        }
        else if (_dashActive && _canDash)
        {
            ThirdPersonController.Instance.canJump = false;
            StartCoroutine(DashRoutine());
        }
    }

    public void ToggleHandLayer(bool state)
    {
        if (state) anim.SetLayerWeight(1, 1);
        else anim.SetLayerWeight(1, 0);
    }

    IEnumerator M2SmashCooldown()
    {
        HUDManager.hudManager.StartCooldown(_smashGroundCooldown, 1);
        _canSmash = false;
        yield return new WaitForSeconds(_smashGroundCooldown);
        _canSmash = true;
    }
    public void SmashGroundEvent()
    {
        ThirdPersonController.Instance.RotatePlayerToCameraForward();
        M2SmashGround();
    }

    public void SmashGroundPause()
    {
        StartCoroutine(PauseRoutine());
    }

    IEnumerator PauseRoutine()
    {
        controller.enabled = false;
        yield return new WaitForSeconds(0.5f);
        controller.enabled = true;
    }
    void M2SmashGround()
    {
        Vector3 playerPos = ThirdPersonController.Instance.transform.position;
        // 1. Oyuncunun konumunda dairesel bir alandaki tüm colliderlarý topla
        // Ýstersen merkez noktasýný karakterin biraz önüne de alabilirsin: transform.position + transform.forward
        Collider[] hitColliders = Physics.OverlapSphere(playerPos, _smashGroundRadius, attackLayer);

        foreach (var hitCollider in hitColliders)
        {
            // 2. Çarpýlan nesnede düþman scripti var mý kontrol et
            EnemyScript enemy = hitCollider.GetComponent<EnemyScript>();

            if (enemy != null)
            {
                enemy.TakeDamage(_smashGroundDamage);
                if (enemy.GetHealth() > 0) enemy.ApplyStun(_smashGroundStunTime);
                // 3. Hasar ver ve sersemlet (stun)
                

                // Eðer EnemyScript içinde Stun metodun varsa:
                
            }
        }

        smashEffect.transform.parent = null;
        smashEffect.transform.position = playerPos;
        smashEffect.transform.rotation = Quaternion.Euler(90,0,0);
        smashGroundCircle.transform.localScale= new Vector3(_smashGroundRadius*2, 0.01f, _smashGroundRadius * 2);
        var shapeModule = smashGroundParticle.shape;

        // 2. Deðeri bu deðiþken üzerinden güncelle
        shapeModule.radius = _smashGroundRadius;


        StartCoroutine(FadeToTransparent(smashGroundCircle, 1.5f));
        smashGroundParticle.Play();
    }

    IEnumerator SpinRoutine()
    {
        ToggleHandLayer(false);
        _canAttack = false;
        anim.SetFloat("SpinAttackSpeed", _spinSpeed);
        ThirdPersonController.Instance.SetSpeed(_spinPlayerSpeed);
        _canSpin = false;
        anim.SetTrigger("SpinAttack");
        yield return new WaitForSeconds(_spinDuration);
        ThirdPersonController.Instance.SetSpeed(1f);
        ResetMainAttackEvent();
        StartCoroutine(SpinCooldown());
    }

    IEnumerator SpinCooldown()
    {
        HUDManager.hudManager.StartCooldown(_spinCooldown, 1);
        _canSpin = false;
        yield return new WaitForSeconds(_spinCooldown);
        _canSpin = true;
    }
    public void SpinAttackEvent()
    {
        M2SpinAttack();
    }
    void M2SpinAttack()
    {
        Collider[] hitColliders = Physics.OverlapSphere(ThirdPersonController.Instance.transform.position, _smashGroundRadius, attackLayer);

        foreach (var hitCollider in hitColliders)
        {
            // 2. Çarpýlan nesnede düþman scripti var mý kontrol et
            EnemyScript enemy = hitCollider.GetComponent<EnemyScript>();

            if (enemy != null)
            {
                // 3. Hasar ver ve sersemlet (stun)
                enemy.TakeDamage(_spinDamage);

                if(_spinHealthRegen) PlayerHealthManager.Instance.ModifyHealth(PlayerHealthManager.Instance.GetMaxHealth()*3/100);
            }
        }
    }

    void ChangeFOV(float targetFov, float duration)
    {
        // FreeLook referansýný alalým (Daha temiz kod için)
        var freeLook = ThirdPersonController.Instance.freeLook;

        // DOTween.To(Getter, Setter, TargetValue, Duration)
        DOTween.To(() => freeLook.m_Lens.FieldOfView,
                   x => freeLook.m_Lens.FieldOfView = x,
                   targetFov,
                   duration)
               .SetEase(Ease.OutQuad);
    }

    IEnumerator DashRoutine()
    {
        dashCollider.enabled = true;
        ToggleHandLayer(false);
        _isDashing = true;
        if (_dashImmunity) PlayerHealthManager.Instance.SetImmunity(true);
        dashStartPos = ThirdPersonController.Instance.transform.position;
        controller.enabled = false;
        ThirdPersonController.Instance.enabled = false;
        _canDash = false;
        anim.SetTrigger("DashAttack");
        dashParticle.Play();
        ChangeFOV(70, 0.1f);
        // 1. Hedef konumu hesapla (Karakterin baktýðý yönde _dashRange kadar ileri)
        Transform player = ThirdPersonController.Instance.transform;
        
        forward = Camera.main.transform.forward;
        forward.y = 0;
        forward.Normalize();
        player.forward = forward;

        Vector3 targetPosition = player.position + (forward * _dashRange);

        // 2. Bu layer'ý "tersine çevir" (Yani Enemy HARÝÇ her þey)
        int layersToIgnore = (1 << LayerMask.NameToLayer("Enemy")) | (1 << LayerMask.NameToLayer("Player"));
        int layerMask = ~layersToIgnore;
        // 2. DOTween ile hareket ettir
        if (Physics.Raycast(player.position + Vector3.up, forward, out RaycastHit hit, _dashRange, layerMask))
        {
            // Eðer bir þeye çarptýysak, hedefi çarpýþma noktasýnýn 0.5 birim gerisi yapýyoruz
            targetPosition = hit.point - (forward * 0.5f);
            // Yüksekliði karakterin mevcut yüksekliðinde sabitliyoruz (yerin içine girmesin)
            targetPosition.y = player.position.y;
        }
        // .SetEase(Ease.OutQuad) baþlangýçta hýzlý, sonda yavaþ bir dash hissi verir
        player.DOMove(targetPosition, 0.2f)
            .SetEase(Ease.Linear) // Sabit hýz için Linear, daha organik his için OutQuad kullanabilirsin
            .OnComplete(() =>
            {
                // 3. Hareket bittiðinde saldýrýyý tetikle
                player.forward = forward;
                //M2DashAttack();
                ChangeFOV(60, 0.1f);
            });

        // Dash süresi kadar bekle
        yield return new WaitForSeconds(0.21f);
        dashCollider.enabled = false;
        _isDashing = false;
        ThirdPersonController.Instance.enabled = true;
        controller.enabled = true;
        ResetMainAttackEvent();
        StartCoroutine(DashCooldown());
        yield return new WaitForSeconds(1.5f);
        if (_dashImmunity) PlayerHealthManager.Instance.SetImmunity(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isDashing)
        {
            EnemyScript enemy = other.gameObject.GetComponent<EnemyScript>();
            if (enemy != null)
            {
                enemy.TakeDamage(_dashDamage);
                if (enemy.GetHealth() > 0) enemy.ApplyStun(0.5f);
            }
        }
    }

    IEnumerator DashCooldown()
    {
        HUDManager.hudManager.StartCooldown(_dashCooldown, 1);
        _canDash = false;
        yield return new WaitForSeconds(_dashCooldown);
        _canDash = true;

    }
    IEnumerator FadeToTransparent(MeshRenderer mesh, float duration)
    {
        // Mevcut materyalin rengini al
        Material mat = mesh.material;
        Color realColor = mat.GetColor("_BaseColor");
        Color startColor = new Color(realColor.r, realColor.g, realColor.b, 1f);
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f); // Hedef: Tam saydam

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // Zaman ilerledikçe renkler arasýnda yumuþak geçiþ (Lerp) yap
            mat.SetColor("_BaseColor", Color.Lerp(startColor, endColor, elapsed / duration));

            yield return null; // Bir sonraki kareye kadar bekle
        }

        // Deðerin tam sýfýr olduðundan emin ol
        mat.SetColor("_BaseColor", endColor);
    }
    public override void UltimateAttack()
    {
        Shield();
    }

    
    void Shield()
    {
        if (_canUlti)
        {
            _canUlti = false;
            StartCoroutine(ShieldRoutine());
        }
    }

    IEnumerator ShieldRoutine()
    {
        if (_deflectDamage) PlayerHealthManager.Instance.deflectsDamage = true;
        if (_shieldSlowness) ThirdPersonController.Instance.SetSpeed(0.5f);
        else ThirdPersonController.Instance.SetSpeed(1f);
        shield.SetActive(true);
        health.SetImmunity(true);
        yield return new WaitForSeconds(_shieldDuration);
        shield.SetActive(false);
        health.SetImmunity(false);
        ThirdPersonController.Instance.SetSpeed(1f);
        PlayerHealthManager.Instance.deflectsDamage = false;

        StartCoroutine(ShieldCooldown());
    }

    IEnumerator ShieldCooldown()
    {
        HUDManager.hudManager.StartCooldown(_shieldCooldown, 2);
        yield return new WaitForSeconds(_shieldCooldown);
        _canUlti = true;
    }

    private void OnDrawGizmosSelected()
    {
        // ThirdPersonController instance kontrolü (Hata almamak için)
        if (ThirdPersonController.Instance == null) return;

        Vector3 playerPos = ThirdPersonController.Instance.transform.position;
        Quaternion playerRot = ThirdPersonController.Instance.transform.rotation;

        // --- 1. SMASH & SPIN (Dairesel Alan) ---
        // Smash ve Spin ayný yarýçapý kullanýyor gibi göründüðü için sarý bir çember çiziyoruz
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(playerPos, _smashGroundRadius);

        // --- 2. DASH ATTACK (Kutu Alaný) ---
        Gizmos.color = Color.cyan;

        // Kutu çiziminde rotasyonu hesaba katmak için Gizmos matrisini oyuncuya göre ayarla
        Matrix4x4 oldMatrix = Gizmos.matrix; // Eski matrisi yedekle

        // DashCenter hesaplamasýný koddakiyle ayný yapýyoruz: Karakterin önünde
        Vector3 dashCenter = ThirdPersonController.Instance.transform.position - (_dashRange / 2) * ThirdPersonController.Instance.transform.forward + new Vector3(0, 1f, 0.25f);

        // Matrisi ayarla: Pozisyon, Rotasyon, Ölçek
        Gizmos.matrix = Matrix4x4.TRS(dashCenter, playerRot, Vector3.one);

        // DrawWireCube "Full Size" bekler, bu yüzden _dashWidth ve _dashRange deðerlerini direkt yazýyoruz
        // Yüksekliði (Y) 2 birim vererek kutunun yerden yüksekliðini görebilirsin
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(_dashWidth, 2f, _dashRange-0.5f));

        // Matrisi eski haline döndür (Diðer gizmoslarýn bozulmamasý için)
        Gizmos.matrix = oldMatrix;

        // --- 3. MAIN ATTACK (Raycast Hattý) ---
        Gizmos.color = Color.red;
        if (Camera.main != null)
        {
            Vector3 rayStart = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
            Gizmos.DrawRay(rayStart, Camera.main.transform.forward * _attackRange);
        }
    }
}
