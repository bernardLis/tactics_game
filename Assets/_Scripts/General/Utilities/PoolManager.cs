using System.Collections.Generic;
using UnityEngine;

namespace Lis
{
    public abstract class PoolManager<T> : MonoBehaviour where T : MonoBehaviour
    {
        GameObject _prefab;
        List<T> _pool = new();
        protected Transform PoolHolder;

        protected void CreatePool(GameObject prefab, int count = 200)
        {
            _prefab = prefab;
            PoolHolder = new GameObject(prefab.name + " Pool").transform;

            _pool = new();
            for (int i = 0; i < count; i++)
            {
                T p = Instantiate(prefab, PoolHolder).GetComponent<T>();
                p.gameObject.SetActive(false);
                _pool.Add(p);
            }
        }

        public T GetObjectFromPool()
        {
            T obj = _pool.Find(o => !o.gameObject.activeSelf);
            if (obj != null) return obj;
            T p = Instantiate(_prefab, PoolHolder).GetComponent<T>();
            p.gameObject.SetActive(false);
            _pool.Add(p);
            return p;

        }

        protected List<T> GetActiveObjects()
        {
            return _pool.FindAll(o => o.gameObject.activeSelf);
        }
    }
}