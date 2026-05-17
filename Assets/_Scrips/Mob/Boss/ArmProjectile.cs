using UnityEngine;

public class ArmProjectile : MonoBehaviour
{
    public float speed = 10f;
    public int maxBounce = 10;
    public float lifeTime = 3f;

    private Vector2 direction;
    private Transform target;
    private Boss boss;
    private int bounceCount = 0;

    public void Init(Boss b, Transform t)
    {
        boss = b;
        target = t;
        direction = (target.position - transform.position).normalized;
    }

    void OnEnable()
    {
        CancelInvoke();
        Invoke(nameof(Return), lifeTime);
        bounceCount = 0;
    }

    void Update()
    {
        Move();
        CheckBounce();
    }

    void Move()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    void CheckBounce()
    {
        if (Camera.main == null) return;

        Vector3 view = Camera.main.WorldToViewportPoint(transform.position);

        bool bounced = false;

        if (view.x <= 0f || view.x >= 1f)
        {
            direction.x *= -1;
            bounced = true;
        }

        if (view.y <= 0f || view.y >= 1f)
        {
            direction.y *= -1;
            bounced = true;
        }

        if (bounced)
        {
            bounceCount++;
            transform.position += (Vector3)direction * 0.2f;

            if (bounceCount >= maxBounce)
                Return();
        }
    }

    void Return()
    {
        if (boss != null)
        {
            boss.ReturnArm();
            boss = null;
        }

        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController p = other.GetComponent<PlayerController>();
            if (p != null)
                p.TakeDamage(1);

            Return();
        }
    }
}