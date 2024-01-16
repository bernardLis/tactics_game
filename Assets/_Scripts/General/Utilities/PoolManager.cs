using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PoolManager<T> : MonoBehaviour where T : MonoBehaviour
{
    GameObject _prefab;
    List<T> _pool = new();
    Transform _poolHolder;

    protected void CreatePool(GameObject prefab, int count = 200)
    {
        _prefab = prefab;
        _poolHolder = new GameObject(prefab.name + " Pool").transform;

        _pool = new();
        for (int i = 0; i < count; i++)
        {
            T p = Instantiate(prefab, _poolHolder).GetComponent<T>();
            p.gameObject.SetActive(false);
            _pool.Add(p);
        }
    }

    public T GetObjectFromPool()
    {
        T obj = _pool.Find(o => !o.gameObject.activeSelf);
        if (obj == null)
        {
            T p = Instantiate(_prefab, _poolHolder).GetComponent<T>();
            p.gameObject.SetActive(false);
            _pool.Add(p);
            return p;
        }

        return obj;
    }

    protected List<T> GetActiveObjects()
    {
        return _pool.FindAll(o => o.gameObject.activeSelf);
    }

}