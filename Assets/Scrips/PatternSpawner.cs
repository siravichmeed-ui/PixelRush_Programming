using UnityEngine;
using System.Collections;

public class PatternSpawner : MonoBehaviour
{
    [Header("Pattern")]
    public PatternData[] easy;
    public PatternData[] medium;
    public PatternData[] hard;

    [Header("Boss")]
    public GameObject bossPrefab;
    public float bossDistance = 300f;
    public Vector2 bossSpawnPosition =
        new Vector2(10f, 0f);

    [Header("Item")]
    public GameObject itemPrefab;
    public float itemDelay = 2f;
    public Transform[] itemSpawnPoints;

    private bool bossSpawned = false;

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            float distance =
                GameManager.Instance.distance;

            if (!bossSpawned &&
                distance >= bossDistance)
            {
                SpawnBoss();

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
                Random.Range(0, medium.Length)
            ];
        }

        return hard[
            Random.Range(0, hard.Length)
        ];
    }

    IEnumerator SpawnPattern(
        PatternData pattern,
        float distance
    )
    {
        foreach (var rule in pattern.spawnRules)
        {
            if (Random.value > rule.spawnChance)
                continue;

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

    void SpawnBoss()
    {
        bossSpawned = true;

        ObjectPool.Instance.Spawn(
            bossPrefab,
            bossSpawnPosition,
            Quaternion.identity
        );

        GameManager.Instance.EnterBossPhase();

        StartCoroutine(ItemLoop());
    }

    IEnumerator ItemLoop()
    {
        while (true)
        {
            if (Boss.Instance == null ||
                Boss.Instance.IsDead())
            {
                yield break;
            }

            SpawnItem();

            yield return new WaitForSeconds(
                itemDelay
            );
        }
    }

    void SpawnItem()
    {
        if (itemSpawnPoints == null ||
            itemSpawnPoints.Length == 0)
        {
            return;
        }

        Transform point =
            itemSpawnPoints[
                Random.Range(
                    0,
                    itemSpawnPoints.Length
                )
            ];

        Vector2 pos = point.position;

        GameObject obj =
            ObjectPool.Instance.Spawn(
                itemPrefab,
                pos,
                Quaternion.identity
            );

        Rigidbody2D rb =
            obj.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity =
                Vector2.left * 5f;
        }
    }
}