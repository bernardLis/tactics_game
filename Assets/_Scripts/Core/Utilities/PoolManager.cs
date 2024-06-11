using System.Collections.Generic;
using Lis.Units;
using UnityEngine;

namespace Lis.Core.Utilities
{
    public abstract class PoolManager<T> : MonoBehaviour where T : MonoBehaviour
    {
        private GameObject _prefab;
        protected List<T> Pool = new();
        protected Transform PoolHolder;

        protected void CreatePool(GameObject prefab, int count = 200)
        {
            _prefab = prefab;
            PoolHolder = new GameObject(prefab.name + " Pool").transform;

            Pool = new();
            for (int i = 0; i < count; i++) InstantiateNewObject();
        }

        public T GetObjectFromPool()
        {
            return GetInactiveObject();
        }

        private T GetInactiveObject()
        {
            // TODO: bad solution to avoid null reference exception
            List<T> nullObjects = new();
            T inactiveObject = null;
            foreach (T o in Pool)
            {
                if (o == null)
                {
                    nullObjects.Add(o);
                    continue;
                }

                if (o.gameObject.activeSelf) continue;
                inactiveObject = o;
                break;
            }

            foreach (T o in nullObjects)
                Pool.Remove(o);

            if (inactiveObject == null) inactiveObject = InstantiateNewObject();
            return inactiveObject;
        }

        private T InstantiateNewObject()
        {
            T p = Instantiate(_prefab, PoolHolder).GetComponent<T>();
            if (p.TryGetComponent(out UnitController unit))
                unit.InitializeGameObject();

            p.gameObject.SetActive(false);
            Pool.Add(p);

            return p;
        }

        protected List<T> GetActiveObjects()
        {
            return Pool.FindAll(o => o.gameObject.activeSelf);
        }
    }
}