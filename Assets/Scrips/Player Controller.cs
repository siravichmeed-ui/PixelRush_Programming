using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private HeartUI heartUI;

    [SerializeField] private GameObject gameOverUI;

    [SerializeField] private BuffUI buffUI;

    // ================= COMPONENT =================
    [Header("Component")]
    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private Animator anim;

    [SerializeField] private CapsuleCollider2D col;

    [SerializeField] private SpriteRenderer sr;

    // ================= EFFECT =================
    [Header("Effects")]
    [SerializeField] private GameObject healEffect;

    [SerializeField] private GameObject speedEffect;

    [SerializeField] private GameObject immortalEffect;

    // ================= GROUND =================
    [Header("Ground Check")]
    [SerializeField] private Transform feetPos;

    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private float groundDistance = 0.3f;

    // ================= JUMP =================
    [Header("Jump")]
    [SerializeField] private float jumpForce = 12f;

    [SerializeField] private int maxJumpCount = 2;

    // ================= CROUCH =================
    [Header("Crouch")]
    [SerializeField]
    private Vector2 standSize =
        new Vector2(1f, 1.8f);

    [SerializeField]
    private Vector2 crouchSize =
        new Vector2(1f, 1f);

    [SerializeField]
    private Vector2 standOffset =
        new Vector2(0f, 0f);

    [SerializeField]
    private Vector2 crouchOffset =
        new Vector2(0f, -0.4f);

    // ================= HEALTH =================
    [Header("Health")]
    [SerializeField] private int maxHP = 3;

    [SerializeField]
    private float invincibleTime = 1f;

    private int currentHP;

    private bool isDead = false;

    private bool isInvincible = false;

    // ================= ITEM EFFECT =================
    private bool isImmortal = false;

    // ================= SPEED BOOST =================
    private Coroutine speedCoroutine;

    private float speedBoostTimer = 0f;

    private float currentSpeedMultiplier = 1f;

    // ================= IMMORTAL =================
    private Coroutine immortalCoroutine;

    private float immortalTimer = 0f;

    // ================= STATE =================
    private bool isGrounded;

    private int jumpCount;

    void Start()
    {
        currentHP = maxHP;

        if (heartUI != null)
        {
            heartUI.UpdateHearts(currentHP);
        }

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }

        col.size = standSize;

        col.offset = standOffset;

        // 👉 ปิด effect ตอนเริ่ม
        if (healEffect != null)
        {
            healEffect.SetActive(false);
        }

        if (speedEffect != null)
        {
            speedEffect.SetActive(false);
        }

        if (immortalEffect != null)
        {
            immortalEffect.SetActive(false);
        }
    }

    void Update()
    {
        if (isDead)
            return;

        CheckGround();

        HandleJump();

        HandleCrouch();

        HandleInventoryInput();

        UpdateAnimation();
    }

    // ================= INVENTORY =================
    void HandleInventoryInput()
    {
        if (Inventory.Instance == null)
            return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            Inventory.Instance.SelectSlot(0);
        }

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            Inventory.Instance.SelectSlot(1);
        }

        if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            Inventory.Instance.SelectSlot(2);
        }

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            Inventory.Instance.UseSelectedItem();
        }
    }

    // ================= GROUND =================
    void CheckGround()
    {
        bool wasGrounded = isGrounded;

        isGrounded =
            Physics2D.OverlapCircle(
                feetPos.position,
                groundDistance,
                groundLayer
            );

        if (!wasGrounded && isGrounded)
        {
            jumpCount = 0;
        }
    }

    // ================= JUMP =================
    void HandleJump()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (jumpCount < maxJumpCount)
            {
                jumpCount++;

                rb.linearVelocity =
                    new Vector2(
                        rb.linearVelocity.x,
                        0f
                    );

                rb.AddForce(
                    Vector2.up * jumpForce,
                    ForceMode2D.Impulse
                );
            }
        }
    }

    // ================= CROUCH =================
    void HandleCrouch()
    {
        if (
            isGrounded &&
            Keyboard.current.leftCtrlKey
            .wasPressedThisFrame
        )
        {
            anim.SetBool(
                "isCrouching",
                true
            );

            col.size = crouchSize;

            col.offset = crouchOffset;
        }

        if (
            Keyboard.current.leftCtrlKey
            .wasReleasedThisFrame
        )
        {
            anim.SetBool(
                "isCrouching",
                false
            );

            col.size = standSize;

            col.offset = standOffset;
        }
    }

    // ================= DAMAGE =================
    public void TakeDamage(int dmg)
    {
        if (
            isDead ||
            isInvincible ||
            isImmortal
        )
        {
            return;
        }

        isInvincible = true;

        currentHP -= dmg;

        if (heartUI != null)
        {
            heartUI.UpdateHearts(currentHP);
        }

        anim.SetTrigger("hit");

        if (currentHP <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(Invincible());
        }
    }

    IEnumerator Invincible()
    {
        yield return new WaitForSeconds(
            invincibleTime
        );

        isInvincible = false;
    }

    // ================= HEAL =================
    public void Heal(int amount)
    {
        currentHP += amount;

        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }

        if (heartUI != null)
        {
            heartUI.UpdateHearts(currentHP);
        }

        StartCoroutine(
            ShowHealEffect()
        );
    }

    IEnumerator ShowHealEffect()
    {
        if (healEffect == null)
            yield break;

        healEffect.SetActive(true);

        yield return new WaitForSeconds(1f);

        healEffect.SetActive(false);
    }

    // ================= SPEED BOOST =================
    public void SpeedBoost(
     float multiplier,
     float duration
 )
    {
        // 👉 เพิ่มเวลาก่อน
        speedBoostTimer += duration;

        // 👉 ถ้ายังไม่มีบัฟ
        if (speedCoroutine == null)
        {
            currentSpeedMultiplier =
                multiplier;

            GameManager.Instance.speed =
                GameManager.Instance.baseSpeed *
                currentSpeedMultiplier;

            speedCoroutine =
                StartCoroutine(
                    SpeedRoutine()
                );

            Debug.Log("Speed Boost ON");
        }

        // 👉 เปิด effect
        if (speedEffect != null)
        {
            speedEffect.SetActive(true);
        }
    }

    IEnumerator SpeedRoutine()
    {
        while (speedBoostTimer > 0f)
        {
            speedBoostTimer -= Time.deltaTime;

            if (buffUI != null)
            {
                buffUI.UpdateSpeed(
                    speedBoostTimer
                );
            }

            yield return null;
        }

        GameManager.Instance.speed =
            GameManager.Instance.baseSpeed;

        if (speedEffect != null)
        {
            speedEffect.SetActive(false);
        }

        if (buffUI != null)
        {
            buffUI.UpdateSpeed(0f);
        }

        speedCoroutine = null;

        currentSpeedMultiplier = 1f;
    }

    // ================= IMMORTAL =================
    public void SetImmortal(
        float duration
    )
    {
        immortalTimer += duration;

        if (immortalCoroutine == null)
        {
            immortalCoroutine =
                StartCoroutine(
                    ImmortalRoutine()
                );
        }
    }

    IEnumerator ImmortalRoutine()
    {
        isImmortal = true;

        if (immortalEffect != null)
        {
            immortalEffect.SetActive(true);
        }

        while (immortalTimer > 0f)
        {
            immortalTimer -= Time.deltaTime;

            if (buffUI != null)
            {
                buffUI.UpdateImmortal(
                    immortalTimer
                );
            }

            yield return null;
        }

        isImmortal = false;

        if (immortalEffect != null)
        {
            immortalEffect.SetActive(false);
        }

        if (buffUI != null)
        {
            buffUI.UpdateImmortal(0f);
        }

        immortalCoroutine = null;
    }

    // ================= DIE =================
    void Die()
    {
        if (isDead)
            return;

        isDead = true;

        anim.SetTrigger("die");

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }

        GameManager.Instance.StopGame();
    }

    // ================= ANIMATION =================
    void UpdateAnimation()
    {
        anim.SetBool(
            "isGrounded",
            isGrounded
        );

        anim.SetFloat(
            "yVelocity",
            rb.linearVelocity.y
        );
    }

    // ================= ATTACK =================
    public void PlayAttack()
    {
        anim.SetTrigger("attack");
    }

    // ================= DEBUG =================
    private void OnDrawGizmosSelected()
    {
        if (feetPos != null)
        {
            Gizmos.color = Color.red;

            Gizmos.DrawWireSphere(
                feetPos.position,
                groundDistance
            );
        }
    }
}