using UnityEngine;

public class EnemyThrower : MonoBehaviour
{
    private SpriteRenderer sr;
    private float timer;

    [Header("Throw")]
    [SerializeField] private GameObject rockPrefab;

    [SerializeField] private Transform throwPoint;

    [SerializeField] private float throwCooldown = 2f;

    [Header("Throw Force")]
    [SerializeField] private Vector2 throwForce = new Vector2(2f, 5f);

    [Header("Speed Sync")]
    [SerializeField] private float speedMultiplier = 0.3f;

    // Sound
    [Header("SFX")]
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip throwSFX;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= throwCooldown)
        {
            timer = 0f;

            ThrowRock();
        }
    }

    void ThrowRock()
    {
        if (rockPrefab == null ||
            throwPoint == null)
        {
            return;
        }

        if (sr != null && sr.isVisible)
        {
            PlaySFX(throwSFX, 0.8f);
        }

        GameObject rock =
            ObjectPool.Instance.Spawn(
                rockPrefab,
                throwPoint.position,
                Quaternion.identity
            );

        Rigidbody2D rb =
            rock.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            // 👉 reset physics
            rb.linearVelocity =
                Vector2.zero;

            rb.angularVelocity = 0f;

            float randomX = Random.Range(-20f, -20f);

            float randomY = Random.Range(4f, 7f);

            // sync กับ game speed
            float extraSpeed =
                GameManager.Instance.speed *
                speedMultiplier;

            Vector2 force =
                new Vector2(
                    randomX + extraSpeed,
                    randomY
                );

            rb.AddForce(
                force,
                ForceMode2D.Impulse
            );

            // 👉 random spin
            rb.angularVelocity =
                Random.Range(-200f, 200f);
        }
    }

    void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (audioSource == null || clip == null)
        {
            return;
        }

        audioSource.PlayOneShot(clip, volume);
    }
}