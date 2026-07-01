using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; }

    private Dictionary<int, Queue<GameObject>> _poolDictionary = new Dictionary<int, Queue<GameObject>>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public GameObject GetFromPool(GameObject prefab, Vector3 position, Quaternion rotation = default)
    {
        int poolKey = prefab.GetInstanceID();

        if (!_poolDictionary.ContainsKey(poolKey))
        {
            _poolDictionary.Add(poolKey, new Queue<GameObject>());
        }

        if (_poolDictionary[poolKey].Count > 0)
        {
            GameObject obj = _poolDictionary[poolKey].Dequeue();
            obj.transform.SetPositionAndRotation(position, rotation);
            obj.SetActive(true);

            if (obj.TryGetComponent(out AnimalEntity entity))
                entity.ResetForPool();

            return obj;
        }

        GameObject newObj = Instantiate(prefab, position, rotation, transform);
        PoolableItem poolItem = newObj.AddComponent<PoolableItem>();
        poolItem.poolKey = poolKey;

        return newObj;
    }

    public void ReturnToPool(GameObject obj)
    {
        if (obj.TryGetComponent(out PoolableItem poolItem))
        {
            obj.SetActive(false);
            _poolDictionary[poolItem.poolKey].Enqueue(obj);
        }
        else
        {
            Destroy(obj);
        }
    }
}

public class PoolableItem : MonoBehaviour
{
    public int poolKey;
}