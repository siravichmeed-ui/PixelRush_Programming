using UnityEngine;

[System.Serializable]
public class SpawnRule
{
    public GameObject prefab;

    [Header("Spawn Mode")]
    public SpawnMode mode = SpawnMode.RandomAll;

    [Header("Fix Lane (ใช้เมื่อ mode = Fixed)")]
    public int fixedIndex = 0;

    [Header("Custom Points (ใช้เมื่อ mode = CustomSet)")]
    public Transform[] customPoints;

    [Header("Chance")]
    [Range(0f, 1f)]
    public float spawnChance = 1f;
}

public enum SpawnMode
{
    RandomAll,
    Fixed,
    CustomSet
}

[System.Serializable]
public class PatternData
{
    public string name;

    [Header("Default Spawn Points (Lane หลัก)")]
    public Transform[] spawnPoints;

    [Header("Spawn Rules")]
    public SpawnRule[] spawnRules;

    [Header("Delay")]
    public float delay = 0.5f;
}