using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ItemMove : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 5f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        // 👉 reset physics ทุกครั้ง
        rb.linearVelocity = Vector2.zero;

        rb.angularVelocity = 0f;

        rb.rotation = 0f;

        // 👉 วิ่งซ้าย
        rb.linearVelocity =
            Vector2.left * moveSpeed;
    }

    void OnDisable()
    {
        rb.linearVelocity = Vector2.zero;

        rb.angularVelocity = 0f;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 👉 หลุดจอ
        if (other.CompareTag("Destroy Obstacle"))
        {
            gameObject.SetActive(false);
        }

        // 👉 เก็บ item
        if (other.CompareTag("Player"))
        {
            gameObject.SetActive(false);
        }
    }
}