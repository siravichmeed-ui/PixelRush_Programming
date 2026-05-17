using UnityEngine;

[System.Serializable]
public class ItemSpawnData
{
    public GameObject prefab;

    [Range(1, 100)]
    public int weight = 1;
}