using UnityEngine;

public class RockProjectile : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float lifeTime = 5f;

    void OnEnable()
    {
        CancelInvoke();
        Invoke(nameof(Disable), lifeTime);
    }

    void Disable()
    {
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Player"))
        {
            PlayerController player = other.collider.GetComponent<PlayerController>();
            if (player != null)
                player.TakeDamage(damage);

            Disable();
            return;
        }

        if (((1 << other.gameObject.layer) & groundLayer) != 0)
        {
            Disable();
        }
    }
}