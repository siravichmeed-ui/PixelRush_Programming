using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyChaser : MonoBehaviour
{
    [SerializeField] private Transform target;

    [Header("Move")]
    [SerializeField]
    private float moveMultiplier = 1f;

    [SerializeField]
    private float lifeTime = 8f;

    private Rigidbody2D rb;

    private Vector2 direction;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        CancelInvoke();

        Invoke(nameof(Disable), lifeTime);

        rb.linearVelocity = Vector2.zero;

        if (target == null)
        {
            GameObject obj =
                GameObject.FindGameObjectWithTag(
                    "Player"
                );

            if (obj != null)
            {
                target = obj.transform;
            }
        }

        if (target != null)
        {
            direction =
                (
                    (Vector2)target.position -
                    rb.position
                ).normalized;
        }
    }

    void FixedUpdate()
    {
        // 👉 sync กับ game speed
        float moveSpeed =
            GameManager.Instance.speed *
            moveMultiplier;

        rb.linearVelocity =
            new Vector2(
                direction.x * moveSpeed,
                rb.linearVelocity.y
            );
    }

    void Disable()
    {
        rb.linearVelocity = Vector2.zero;

        gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(
        Collision2D other
    )
    {
        if (other.collider.CompareTag("Player"))
        {
            PlayerController player =
                other.collider.GetComponent
                <PlayerController>();

            if (player != null)
            {
                player.TakeDamage(1);
            }

            Disable();
        }
    }
}