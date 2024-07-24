using Lis.Units.Enemy;
using UnityEngine;
using UnityEngine.Pool;

namespace Lis.Arena
{
    public class EnemyPool : MonoBehaviour
    {
        readonly bool _collectionCheck = true;
        readonly int _defaultCapacity = 100;
        readonly int _maxCapacity = 1000;

        Enemy _enemy;

        IObjectPool<EnemyController> _pool;

        public void Initialize(Enemy enemy)
        {
            _enemy = enemy;
            _pool = new ObjectPool<EnemyController>(
                CreateEnemy,
                OnTakeFromPool,
                OnReturnToPool,
                OnDestroyPoolObject,
                _collectionCheck,
                _defaultCapacity,
                _maxCapacity
            );
        }

        public EnemyController Get()
        {
            return _pool.Get();
        }

        public void Return(EnemyController enemyController)
        {
            _pool.Release(enemyController);
        }

        EnemyController CreateEnemy()
        {
            Enemy instance = Instantiate(_enemy);
            instance.InitializeFight(1);
            GameObject g = Instantiate(instance.Prefab, transform);
            EnemyController ec = g.GetComponent<EnemyController>();

            ec.InitializeGameObject();
            ec.InitializeUnit(instance, 1);
            ec.SetPool(this);
            return ec;
        }

        void OnTakeFromPool(EnemyController enemyController)
        {
            enemyController.gameObject.SetActive(true);
            enemyController.EnableSelf();
        }

        void OnReturnToPool(EnemyController enemyController)
        {
            enemyController.Unit.CurrentHealth.SetValue(enemyController.Unit.MaxHealth.BaseValue);
            enemyController.DisableSelf();
        }

        void OnDestroyPoolObject(EnemyController enemyController)
        {
            Destroy(enemyController.gameObject);
        }
    }
}