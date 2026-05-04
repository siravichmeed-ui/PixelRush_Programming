using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;

    private Dictionary<string, Queue<GameObject>> poolDict;

    void Awake()
    {
        Instance = this;
        poolDict = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> queue = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                queue.Enqueue(obj);
            }

            poolDict.Add(pool.tag, queue);
        }
    }

    public GameObject Spawn(string tag, Vector3 pos, Quaternion rot)
    {
        if (!poolDict.ContainsKey(tag))
        {
            Debug.LogError("No pool: " + tag);
            return null;
        }

        GameObject obj = poolDict[tag].Dequeue();

        obj.SetActive(true);
        obj.transform.position = pos;
        obj.transform.rotation = rot;

        poolDict[tag].Enqueue(obj);

        return obj;
    }
}