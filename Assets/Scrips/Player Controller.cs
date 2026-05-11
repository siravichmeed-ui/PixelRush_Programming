using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private HeartUI heartUI;
    [SerializeField] private GameObject gameOverUI;

    [Header("Component")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator anim;
    [SerializeField] private CapsuleCollider2D col;
    [SerializeField] private SpriteRenderer sr;

    [Header("Ground Check")]
    [SerializeField] private Transform feetPos;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundDistance = 0.3f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private int maxJumpCount = 2;

    [Header("Crouch")]
    [SerializeField] private Vector2 standSize = new Vector2(1f, 1.8f);
    [SerializeField] private Vector2 crouchSize = new Vector2(1f, 1f);
    [SerializeField] private Vector2 standOffset = new Vector2(0f, 0f);
    [SerializeField] private Vector2 crouchOffset = new Vector2(0f, -0.4f);

    [Header("Health")]
    [SerializeField] private int maxHP = 3;
    [SerializeField] private float invincibleTime = 1f;

    private int currentHP;

    private bool isDead = false;
    private bool isInvincible = false;

    // ================= ITEM EFFECT =================
    private bool isImmortal = false;

    // ================= STATE =================
    private bool isGrounded;
    private int jumpCount;

    void Start()
    {
        currentHP = maxHP;

        heartUI.UpdateHearts(currentHP);

        if (gameOverUI != null)
            gameOverUI.SetActive(false);

        col.size = standSize;
        col.offset = standOffset;
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

        // 👉 เลือกช่อง
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

        // 👉 ใช้ item
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            Inventory.Instance.UseSelectedItem();
        }
    }

    // ================= GROUND =================
    void CheckGround()
    {
        bool wasGrounded = isGrounded;

        isGrounded = Physics2D.OverlapCircle(
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

                rb.linearVelocity = new Vector2(
                    rb.linearVelocity.x,
                    0f
                );

                rb.AddForce(
                    Vector2.up * jumpForce,
                    ForceMode2D.Impulse
                );

                Debug.Log("Jump : " + jumpCount);
            }
        }
    }

    // ================= CROUCH =================
    void HandleCrouch()
    {
        if (isGrounded &&
            Keyboard.current.leftCtrlKey.wasPressedThisFrame)
        {
            anim.SetBool("isCrouching", true);

            col.size = crouchSize;
            col.offset = crouchOffset;
        }

        if (Keyboard.current.leftCtrlKey.wasReleasedThisFrame)
        {
            anim.SetBool("isCrouching", false);

            col.size = standSize;
            col.offset = standOffset;
        }
    }

    // ================= DAMAGE =================
    public void TakeDamage(int dmg)
    {
        if (isDead || isInvincible || isImmortal)
            return;

        isInvincible = true;

        currentHP -= dmg;

        Debug.Log("HP : " + currentHP);

        heartUI.UpdateHearts(currentHP);

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
        yield return new WaitForSeconds(invincibleTime);

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

        heartUI.UpdateHearts(currentHP);

        Debug.Log("Heal : " + amount);
    }

    // ================= SPEED BOOST =================
    public void SpeedBoost(
        float multiplier,
        float duration
    )
    {
        StopCoroutine("SpeedRoutine");

        StartCoroutine(
            SpeedRoutine(multiplier, duration)
        );
    }

    IEnumerator SpeedRoutine(
        float multiplier,
        float duration
    )
    {
        // 👉 จำ speed ปัจจุบัน
        float oldSpeed =
            GameManager.Instance.speed;

        // 👉 เพิ่ม speed
        GameManager.Instance.speed *= multiplier;

        Debug.Log("Speed Boost ON");

        // 👉 รอตามเวลา
        yield return new WaitForSeconds(duration);

        // 👉 คืนค่าเดิม
        GameManager.Instance.speed = oldSpeed;

        Debug.Log("Speed Boost OFF");
    }

    // ================= IMMORTAL =================
    public void SetImmortal(float duration)
    {
        StopCoroutine("ImmortalRoutine");

        StartCoroutine(
            ImmortalRoutine(duration)
        );
    }

    IEnumerator ImmortalRoutine(float duration)
    {
        isImmortal = true;

        Debug.Log("Immortal ON");

        yield return new WaitForSeconds(duration);

        isImmortal = false;

        Debug.Log("Immortal OFF");
    }

    // ================= DIE =================
    void Die()
    {
        if (isDead)
            return;

        isDead = true;

        Debug.Log("Game Over");

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
        anim.SetBool("isGrounded", isGrounded);

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