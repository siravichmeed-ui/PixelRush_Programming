using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour
{
    public static Boss Instance;

    [Header("References")]
    [SerializeField] private Transform player;

    // ================= ARM =================
    [Header("Arm")]
    [SerializeField] private GameObject armObject;

    [SerializeField] private Animator armAnim;

    // ================= SHOOT =================
    [Header("Shoot Point")]
    [SerializeField] private Transform shootPos;

    [Header("Projectile Arm")]
    [SerializeField] private GameObject armPrefab;

    // ================= ATTACK =================
    [Header("Attack")]
    [SerializeField] private float attackCooldown = 2f;

    // ================= AIM =================
    [Header("Aim")]
    [SerializeField] private float rotateSpeed = 10f;

    [SerializeField] private float minAngle = -80f;

    [SerializeField] private float maxAngle = 80f;

    [SerializeField] private float angleOffset = 0f;

    // ================= HP =================
    [Header("HP")]
    [SerializeField] private int maxHP = 10;

    // ================= EFFECT =================
    [Header("Effect")]
    [SerializeField] private Animator anim;

    [SerializeField] private SpriteRenderer sr;

    [SerializeField] private float destroyDelay = 3f;

    // ================= UI =================
    private BossUI bossUI;

    // ================= STATE =================
    private int currentHP;

    private float attackTimer;

    private int phase = 1;

    private bool isArmOut = false;

    private bool isDead = false;

    private Coroutine flashCoroutine;

    // ================= SFX =================

    [Header("SFX")]
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip attackSFX;

    [SerializeField] private AudioClip hitSFX;

    // ================= UNITY =================
    void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        Instance = this;

        currentHP = maxHP;

        attackTimer = 0f;

        phase = 1;

        isDead = false;

        isArmOut = false;

        attackCooldown = 2f;

        // 👉 reset color
        if (sr != null)
        {
            sr.color = Color.white;
        }

        // 👉 get boss ui
        bossUI = BossUI.Instance;

        // 👉 show hp ui
        if (bossUI != null)
        {
            bossUI.Show(maxHP);
        }

        FindPlayer();
    }

    void Update()
    {
        if (isDead)
            return;

        if (player == null)
        {
            FindPlayer();

            return;
        }

        HandleAttack();

        UpdatePhase();
    }

    void LateUpdate()
    {
        if (isDead)
            return;

        AimShootPos();
    }

    // ================= FIND PLAYER =================
    void FindPlayer()
    {
        GameObject obj =
            GameObject.FindGameObjectWithTag(
                "Player"
            );

        if (obj != null)
        {
            player = obj.transform;
        }
    }

    // ================= AIM =================
    void AimShootPos()
    {
        if (
            isArmOut ||
            shootPos == null ||
            player == null
        )
        {
            return;
        }

        Vector2 dir =
            player.position -
            shootPos.position;

        float angle =
            Mathf.Atan2(
                dir.y,
                dir.x
            ) * Mathf.Rad2Deg;

        angle += angleOffset;

        angle = Mathf.Clamp(
            angle,
            minAngle,
            maxAngle
        );

        Quaternion targetRot =
            Quaternion.Euler(
                0,
                0,
                angle
            );

        shootPos.localRotation =
            Quaternion.Lerp(
                shootPos.localRotation,
                targetRot,
                Time.deltaTime *
                rotateSpeed
            );
    }

    // ================= ATTACK =================
    void HandleAttack()
    {
        if (isArmOut)
            return;

        attackTimer += Time.deltaTime;

        if (attackTimer >= attackCooldown)
        {
            attackTimer = 0f;

            if (armAnim != null)
            {
                armAnim.SetTrigger(
                    "attack"
                );
            }
        }
    }

    public void ShootArm()
    {
        if (
            isDead ||
            isArmOut ||
            shootPos == null ||
            armPrefab == null
        )
        {
            return;
        }

        isArmOut = true;

        PlaySFX(attackSFX, 0.5f);

        if (armObject != null)
        {
            armObject.SetActive(false);
        }

        GameObject armObj =
            ObjectPool.Instance.Spawn(
                armPrefab,
                shootPos.position,
                Quaternion.identity
            );

        ArmProjectile proj =
            armObj.GetComponent<ArmProjectile>();

        if (proj != null)
        {
            proj.Init(
                this,
                player
            );
        }
    }

    public void ReturnArm()
    {
        isArmOut = false;

        if (
            armObject != null &&
            !isDead
        )
        {
            armObject.SetActive(true);
        }
    }

    // ================= PHASE =================
    void UpdatePhase()
    {
        float hpPercent =
            (float)currentHP / maxHP;

        // 👉 phase 2
        if (
            hpPercent <= 0.6f &&
            phase == 1
        )
        {
            phase = 2;

            attackCooldown = 1.2f;

            Debug.Log(
                "Boss Phase 2"
            );
        }

        // 👉 phase 3
        if (
            hpPercent <= 0.3f &&
            phase == 2
        )
        {
            phase = 3;

            attackCooldown = 0.7f;

            Debug.Log(
                "Boss Phase 3"
            );
        }
    }

    // ================= DAMAGE =================
    public void TakeDamage(int dmg)
    {
        if (isDead)
            return;

        currentHP -= dmg;

        if (currentHP < 0)
        {
            currentHP = 0;
        }

        Debug.Log(
            "Boss HP : " +
            currentHP
        );

        // 👉 update hp ui
        if (bossUI != null)
        {
            bossUI.UpdateHP(currentHP);
        }

        // 👉 hit animation
        if (anim != null)
        {
            anim.SetTrigger("hit");

            PlaySFX(hitSFX, 0.7f);
        }

        // 👉 flash effect
        if (flashCoroutine != null)
        {
            StopCoroutine(
                flashCoroutine
            );
        }

        flashCoroutine =
            StartCoroutine(
                HitFlash()
            );

        // 👉 die
        if (currentHP <= 0)
        {
            Die();
        }
    }

    // ================= HIT FLASH =================
    IEnumerator HitFlash()
    {
        if (sr == null)
            yield break;

        Color hitColor =
            new Color(
                1f,
                0.3f,
                0.3f
            );

        // 👉 flash 3 รอบ
        for (int i = 0; i < 3; i++)
        {
            sr.color = hitColor;

            yield return new WaitForSeconds(
                0.08f
            );

            sr.color = Color.white;

            yield return new WaitForSeconds(
                0.08f
            );
        }

        sr.color = Color.white;
    }

    // ================= DIE =================
    void Die()
    {
        if (isDead)
            return;

        isDead = true;

        Debug.Log("Boss Dead");

        // 👉 hide hp ui
        if (bossUI != null)
        {
            bossUI.Hide();
        }

        // 👉 stop flash
        if (flashCoroutine != null)
        {
            StopCoroutine(
                flashCoroutine
            );
        }

        // 👉 reset color
        if (sr != null)
        {
            sr.color = Color.white;
        }

        // 👉 ปิด arm
        if (armObject != null)
        {
            armObject.SetActive(false);
        }

        // 👉 หยุด animation attack
        if (armAnim != null)
        {
            armAnim.enabled = false;
        }

        // 👉 เล่น animation ตาย
        if (anim != null)
        {
            anim.SetTrigger("die");
        }

        // 👉 เข้า endless
        if (GameManager.Instance != null)
        {
            GameManager.Instance
                .BossDefeated();

            PatternSpawner spawner =
            Object.FindFirstObjectByType<PatternSpawner>();

            if (spawner != null)
            {
                spawner.PlayNormalMusic();
            }
        }

        StartCoroutine(
            DisableBoss()
        );
    }

    IEnumerator DisableBoss()
    {
        yield return new WaitForSeconds(
            destroyDelay
        );

        gameObject.SetActive(false);
    }

    public bool IsDead()
    {
        return isDead;
    }

    // ================= SFX =================
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (audioSource == null || clip == null)
        {
            return;
        }

        audioSource.PlayOneShot(clip, volume);
    }
}