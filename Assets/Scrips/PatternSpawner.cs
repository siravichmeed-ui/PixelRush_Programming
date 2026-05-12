using UnityEngine;
using System.Collections;

public class PatternSpawner : MonoBehaviour
{
    [Header("Pattern")]
    public PatternData[] easy;

    public PatternData[] medium;

    public PatternData[] hard;

    // ================= BOSS =================
    [Header("Boss")]
    public GameObject bossPrefab;

    public float bossDistance = 300f;

    public Vector2 bossSpawnPosition =
        new Vector2(10f, 0f);

    // ================= ITEM =================
    [Header("Normal Item")]
    public GameObject[] normalItems;

    [Header("Boss Item")]
    public GameObject[] bossItems;

    [Header("Item Setting")]
    public float itemDelay = 2f;

    public Transform[] itemSpawnPoints;

    private bool bossSpawned = false;

    void Start()
    {
        StartCoroutine(SpawnLoop());

        StartCoroutine(ItemLoop());
    }

    // ================= MAIN LOOP =================
    IEnumerator SpawnLoop()
    {
        while (true)
        {
            float distance =
                GameManager.Instance.distance;

            // 👉 spawn boss
            if (!bossSpawned &&
                distance >= bossDistance)
            {
                SpawnBoss();

                // 👉 หยุด wave ปกติ
                yield break;
            }

            PatternData pattern =
                GetPattern(distance);

            yield return StartCoroutine(
                SpawnPattern(
                    pattern,
                    distance
                )
            );

            yield return new WaitForSeconds(1f);
        }
    }

    // ================= GET PATTERN =================
    PatternData GetPattern(float distance)
    {
        if (distance < 100f)
        {
            return easy[
                Random.Range(0, easy.Length)
            ];
        }

        if (distance < 200f)
        {
            return medium[
                Random.Range(
                    0,
                    medium.Length
                )
            ];
        }

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
            if (Random.value >
                rule.spawnChance)
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
        if (defaultPoints == null ||
            defaultPoints.Length == 0)
        {
            return null;
        }

        switch (rule.mode)
        {
            case SpawnMode.RandomAll:

                return defaultPoints[
                    Random.Range(
                        0,
                        defaultPoints.Length
                    )
                ];

            case SpawnMode.Fixed:

                int index = Mathf.Clamp(
                    rule.fixedIndex,
                    0,
                    defaultPoints.Length - 1
                );

                return defaultPoints[index];

            case SpawnMode.CustomSet:

                if (rule.customPoints != null &&
                    rule.customPoints.Length > 0)
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
        bossSpawned = true;

        ObjectPool.Instance.Spawn(
            bossPrefab,
            bossSpawnPosition,
            Quaternion.identity
        );

        GameManager.Instance.EnterBossPhase();

        Debug.Log("Boss Spawn");
    }

    // ================= ITEM LOOP =================
    IEnumerator ItemLoop()
    {
        while (true)
        {
            SpawnItem();

            yield return new WaitForSeconds(
                itemDelay
            );
        }
    }

    // ================= SPAWN ITEM =================
    void SpawnItem()
    {
        if (itemSpawnPoints == null ||
            itemSpawnPoints.Length == 0)
        {
            return;
        }

        GameObject[] currentPool;

        // 👉 ก่อน boss
        if (!GameManager.Instance.isBossPhase)
        {
            currentPool = normalItems;
        }
        // 👉 ตอน boss
        else
        {
            currentPool = bossItems;
        }

        if (currentPool == null ||
            currentPool.Length == 0)
        {
            return;
        }

        // 👉 สุ่มตำแหน่ง
        Transform point =
            itemSpawnPoints[
                Random.Range(
                    0,
                    itemSpawnPoints.Length
                )
            ];

        // 👉 สุ่ม item
        GameObject randomItem =
            currentPool[
                Random.Range(
                    0,
                    currentPool.Length
                )
            ];

        Vector2 pos =
            point.position;

        ObjectPool.Instance.Spawn(
            randomItem,
            pos,
            Quaternion.identity
        );
    }
}