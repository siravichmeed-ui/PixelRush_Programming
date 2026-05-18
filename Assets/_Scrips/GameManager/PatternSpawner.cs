using UnityEngine;
using System.Collections;

public class PatternSpawner : MonoBehaviour
{
    [Header("Music")]
    [SerializeField] private AudioSource musicSource;

    [SerializeField] private AudioClip normalMusic;

    [SerializeField] private AudioClip bossMusic;

    // ================= NORMAL PATTERN =================
    [Header("Pattern")]
    public PatternData[] easy;

    public PatternData[] medium;

    public PatternData[] hard;

    // ================= ENDLESS PATTERN =================
    [Header("Endless")]
    public PatternData[] endless;

    // ================= BOSS =================
    [Header("Boss")]
    public GameObject bossPrefab;

    public float bossDistance = 300f;

    public Vector2 bossSpawnPosition =
        new Vector2(10f, 0f);

    // ================= ENDLESS =================
    [Header("Endless Setting")]
    public float endlessDelay = 5f;

    // ================= ENDLESS SCALING =================
    [Header("Endless Scaling")]
    public float minSpawnDelay = 0.15f;

    public float endlessSpawnRate = 0.003f;

    public float extraSpawnChance = 0.002f;

    // ================= ITEM =================
    [Header("Normal Item")]
    public ItemSpawnData[] normalItems;

    [Header("Boss Item")]
    public ItemSpawnData[] bossItems;

    // ================= ITEM COOLDOWN =================
    [Header("Item Cooldown")]
    public float itemDelay = 2f;

    public float bossItemDelay = 5f;

    // ================= ITEM SPAWN CHANCE =================
    [Header("Item Spawn Chance")]
    [Range(0f, 1f)]
    public float itemSpawnChance = 0.4f;

    [Range(0f, 1f)]
    public float bossItemSpawnChance = 0.8f;

    // ================= ITEM SPAWN POINT =================
    [Header("Spawn Point")]
    public Transform[] itemSpawnPoints;

    // ================= STATE =================
    private bool bossSpawned = false;

    private bool waitingEndless = false;

    void Start()
    {
        StartCoroutine(SpawnLoop());

        StartCoroutine(ItemLoop());
    }

    public void PlayNormalMusic()
    {
        PlayLoopMusic(normalMusic);
    }

    // ================= MAIN LOOP =================
    IEnumerator SpawnLoop()
    {
        while (true)
        {
            float distance =
                GameManager.Instance.distance;

            // ================= SPAWN BOSS =================
            if (!bossSpawned &&
                distance >= bossDistance)
            {
                SpawnBoss();

                bossSpawned = true;
            }

            // ================= BOSS PHASE =================
            if (GameManager.Instance.isBossPhase)
            {
                yield return null;

                continue;
            }

            // ================= WAIT ENDLESS =================
            if (
                !waitingEndless &&
                GameManager.Instance.isEndlessPhase
            )
            {
                waitingEndless = true;

                Debug.Log(
                    "WAIT ENDLESS..."
                );

                yield return new WaitForSeconds(
                    endlessDelay
                );

                Debug.Log(
                    "ENDLESS START"
                );
            }

            PatternData pattern =
                GetPattern(distance);

            if (pattern != null)
            {
                yield return StartCoroutine(
                    SpawnPattern(
                        pattern,
                        distance
                    )
                );
            }

            // ================= SPAWN SPEED =================
            float delay = 1f;

            // 👉 endless ยิ่งไกลยิ่งไว
            if (GameManager.Instance.isEndlessPhase)
            {
                float endlessDistance =
                    distance - bossDistance;

                delay -=
                    endlessDistance *
                    endlessSpawnRate;

                // 👉 กัน delay ติดลบ
                delay = Mathf.Max(
                    minSpawnDelay,
                    delay
                );
            }

            yield return new WaitForSeconds(
                delay
            );
        }
    }

    // ================= GET PATTERN =================
    PatternData GetPattern(float distance)
    {
        // ================= ENDLESS =================
        if (GameManager.Instance.isEndlessPhase)
        {
            // 👉 ใช้ endless pattern
            if (
                endless != null &&
                endless.Length > 0
            )
            {
                return endless[
                    Random.Range(
                        0,
                        endless.Length
                    )
                ];
            }

            // 👉 fallback
            return hard[
                Random.Range(
                    0,
                    hard.Length
                )
            ];
        }

        // ================= EASY =================
        if (distance < 100f)
        {
            return easy[
                Random.Range(
                    0,
                    easy.Length
                )
            ];
        }

        // ================= MEDIUM =================
        if (distance < 200f)
        {
            return medium[
                Random.Range(
                    0,
                    medium.Length
                )
            ];
        }

        // ================= HARD =================
        return hard[
            Random.Range(
                0,
                hard.Length
            )
        ];
    }

    // ================= SPAWN PATTERN =================
    IEnumerator SpawnPattern(
        PatternData pattern,
        float distance
    )
    {
        foreach (var rule in pattern.spawnRules)
        {
            float spawnChance =
                rule.spawnChance;

            // ================= ENDLESS =================
            if (GameManager.Instance.isEndlessPhase)
            {
                float endlessDistance =
                    distance - bossDistance;

                // 👉 ยิ่งไกลยิ่ง spawn เยอะ
                spawnChance +=
                    endlessDistance *
                    extraSpawnChance;

                spawnChance =
                    Mathf.Clamp01(
                        spawnChance
                    );
            }

            if (Random.value > spawnChance)
            {
                continue;
            }

            Transform spawnPoint =
                GetSpawnPoint(
                    rule,
                    pattern.spawnPoints
                );

            if (spawnPoint == null)
                continue;

            Vector2 pos =
                spawnPoint.position;

            GameObject obj =
                ObjectPool.Instance.Spawn(
                    rule.prefab,
                    pos,
                    Quaternion.identity
                );

            Rigidbody2D rb =
                obj.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                float speed =
                    3f + distance * 0.05f;

                // 👉 endless เร็วขึ้น
                if (
                    GameManager.Instance
                    .isEndlessPhase
                )
                {
                    speed *= 1.3f;
                }

                rb.linearVelocity =
                    Vector2.left * speed;
            }

            yield return new WaitForSeconds(
                pattern.delay
            );
        }
    }

    // ================= GET SPAWN POINT =================
    Transform GetSpawnPoint(
        SpawnRule rule,
        Transform[] defaultPoints
    )
    {
        if (
            defaultPoints == null ||
            defaultPoints.Length == 0
        )
        {
            return null;
        }

        switch (rule.mode)
        {
            // ================= RANDOM =================
            case SpawnMode.RandomAll:

                return defaultPoints[
                    Random.Range(
                        0,
                        defaultPoints.Length
                    )
                ];

            // ================= FIXED =================
            case SpawnMode.Fixed:

                int index = Mathf.Clamp(
                    rule.fixedIndex,
                    0,
                    defaultPoints.Length - 1
                );

                return defaultPoints[index];

            // ================= CUSTOM =================
            case SpawnMode.CustomSet:

                if (
                    rule.customPoints != null &&
                    rule.customPoints.Length > 0
                )
                {
                    return rule.customPoints[
                        Random.Range(
                            0,
                            rule.customPoints.Length
                        )
                    ];
                }

                break;
        }

        return defaultPoints[0];
    }

    // ================= SPAWN BOSS =================
    void SpawnBoss()
    {
        ObjectPool.Instance.Spawn(
            bossPrefab,
            bossSpawnPosition,
            Quaternion.identity
        );

        GameManager.Instance.EnterBossPhase();
        PlayLoopMusic(bossMusic, 0.5f);

        Debug.Log("Boss Spawn");
    }

    // ================= ITEM LOOP =================
    IEnumerator ItemLoop()
    {
        while (true)
        {
            float chance;

            // ================= BOSS ITEM =================
            if (GameManager.Instance.isBossPhase)
            {
                chance =
                    bossItemSpawnChance;
            }
            // ================= NORMAL / ENDLESS =================
            else
            {
                chance =
                    itemSpawnChance;
            }

            // 👉 spawn item
            if (Random.value <= chance)
            {
                SpawnItem();
            }

            float delay;

            // ================= BOSS DELAY =================
            if (GameManager.Instance.isBossPhase)
            {
                delay = bossItemDelay;
            }
            // ================= NORMAL DELAY =================
            else
            {
                delay = itemDelay;
            }

            yield return new WaitForSeconds(
                delay
            );
        }
    }

    // ================= SPAWN ITEM =================
    void SpawnItem()
    {
        if (
            itemSpawnPoints == null ||
            itemSpawnPoints.Length == 0
        )
        {
            return;
        }

        ItemSpawnData[] currentPool;

        // ================= BOSS ITEM =================
        if (GameManager.Instance.isBossPhase)
        {
            currentPool = bossItems;
        }
        // ================= NORMAL / ENDLESS =================
        else
        {
            currentPool = normalItems;
        }

        if (
            currentPool == null ||
            currentPool.Length == 0
        )
        {
            return;
        }

        // 👉 random spawn point
        Transform point =
            itemSpawnPoints[
                Random.Range(
                    0,
                    itemSpawnPoints.Length
                )
            ];

        // 👉 random item
        GameObject randomItem =
            GetRandomItem(currentPool);

        if (randomItem == null)
            return;

        ObjectPool.Instance.Spawn(
            randomItem,
            point.position,
            Quaternion.identity
        );
    }

    // ================= WEIGHT RANDOM =================
    GameObject GetRandomItem(
        ItemSpawnData[] pool
    )
    {
        int totalWeight = 0;

        foreach (ItemSpawnData item in pool)
        {
            totalWeight += item.weight;
        }

        int random =
            Random.Range(
                0,
                totalWeight
            );

        int current = 0;

        foreach (ItemSpawnData item in pool)
        {
            current += item.weight;

            if (random < current)
            {
                return item.prefab;
            }
        }

        return null;
    }

    // ================= SOUND =================
    void PlayLoopMusic(AudioClip clip, float volume = 1f)
    {
        if (musicSource == null || clip == null)
            return;

        musicSource.clip = clip;

        musicSource.loop = true;

        musicSource.Play();
    }
}