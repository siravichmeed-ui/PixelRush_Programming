using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    [System.Serializable]
    public class Pool
    {
        public GameObject prefab;

        public int size = 10;
    }

    [Header("Start Pools")]
    [SerializeField] private Pool[] pools;

    private Dictionary<string, List<GameObject>> poolDict =
        new Dictionary<string, List<GameObject>>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);

            return;
        }

        foreach (Pool pool in pools)
        {
            CreatePool(pool.prefab, pool.size);
        }
    }

    // ================= CREATE =================
    void CreatePool(GameObject prefab, int size)
    {
        string key = prefab.name;

        if (poolDict.ContainsKey(key))
            return;

        List<GameObject> list =
            new List<GameObject>();

        for (int i = 0; i < size; i++)
        {
            GameObject obj =
                Instantiate(prefab);

            obj.SetActive(false);

            list.Add(obj);
        }

        poolDict.Add(key, list);
    }

    // ================= SPAWN =================
    public GameObject Spawn(
        GameObject prefab,
        Vector3 pos,
        Quaternion rot
    )
    {
        string key = prefab.name;

        // 👉 ไม่มี pool = สร้างให้อัตโนมัติ
        if (!poolDict.ContainsKey(key))
        {
            Debug.Log(
                "Auto Create Pool : " + key
            );

            CreatePool(prefab, 10);
        }

        List<GameObject> list =
            poolDict[key];

        // 👉 หา object ว่าง
        foreach (GameObject obj in list)
        {
            if (!obj.activeInHierarchy)
            {
                obj.transform.position = pos;
                obj.transform.rotation = rot;

                obj.SetActive(true);

                return obj;
            }
        }

        // 👉 ถ้าเต็ม เพิ่มใหม่อัตโนมัติ
        GameObject newObj =
            Instantiate(prefab);

        newObj.transform.position = pos;
        newObj.transform.rotation = rot;

        newObj.SetActive(true);

        list.Add(newObj);

        return newObj;
    }
}