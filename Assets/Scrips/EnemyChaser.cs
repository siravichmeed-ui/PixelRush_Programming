using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyChaser : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float speed = 6f;
    [SerializeField] private float lifeTime = 8f;

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

        if (target == null)
        {
            GameObject obj = GameObject.FindGameObjectWithTag("Player");
            if (obj != null)
                target = obj.transform;
        }

        if (target != null)
            direction = ((Vector2)target.position - rb.position).normalized;
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(direction.x * speed, rb.velocity.y);
    }

    void Disable()
    {
        rb.velocity = Vector2.zero;
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Player"))
        {
            PlayerController p = other.collider.GetComponent<PlayerController>();
            if (p != null)
                p.TakeDamage(1);

            Disable();
        }
    }
}