using UnityEngine;

public class FireballHoming : MonoBehaviour
{
    [SerializeField] private float speed = 15f;

    private Boss target;
    private int damage;

    public void Init(Boss boss, int dmg)
    {
        target = boss;
        damage = dmg;
    }

    void Update()
    {
        if (target == null || target.IsDead())
        {
            gameObject.SetActive(false);
            return;
        }

        Vector2 dir =
            (
                target.transform.position -
                transform.position
            ).normalized;

        transform.position +=
            (Vector3)(dir * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 👉 ไม่ชน player
        if (other.CompareTag("Player"))
            return;

        // 👉 ไม่ชนกระสุน
        if (other.CompareTag("Obstacle"))
            return;

        // 👉 ชนเฉพาะ boss เท่านั้น
        if (!other.CompareTag("Boss"))
            return;

        Boss boss =
            other.GetComponentInParent<Boss>();

        if (boss != null)
        {
            Debug.Log("BOSS HIT");

            boss.TakeDamage(damage);
        }

        gameObject.SetActive(false);
    }
}