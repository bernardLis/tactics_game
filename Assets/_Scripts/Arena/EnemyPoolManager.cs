using System.Collections.Generic;
using Lis.Core;
using Lis.Core.Utilities;
using Lis.Units.Enemy;
using UnityEngine;

namespace Lis.Arena
{
    public class EnemyPoolManager : Singleton<EnemyPoolManager>
    {
        [SerializeField] GameObject _enemyPoolPrefab;
        readonly Dictionary<string, EnemyPool> _enemyPools = new();

        [SerializeField] Transform _enemyArmyHolder;

        public void Initialize()
        {
            foreach (Enemy enemy in GameManager.Instance.UnitDatabase.GetAllEnemies())
            {
                EnemyPool pool = Instantiate(_enemyPoolPrefab, _enemyArmyHolder.transform).GetComponent<EnemyPool>();
                pool.Initialize(enemy);
                _enemyPools.Add(enemy.Id, pool);
            }
        }

        public EnemyController Get(string enemyId)
        {
            return _enemyPools[enemyId].Get();
        }

        public EnemyPool Return(string enemyId, EnemyController enemyController)
        {
            _enemyPools[enemyId].Return(enemyController);
            return _enemyPools[enemyId];
        }
    }
}