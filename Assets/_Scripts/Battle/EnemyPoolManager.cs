using System.Collections.Generic;
using Lis.Core;
using Lis.Core.Utilities;
using Lis.Units.Enemy;
using UnityEngine;

namespace Lis.Battle
{
    public class EnemyPoolManager : Singleton<EnemyPoolManager>
    {
        [SerializeField] GameObject _enemyPoolPrefab;
        Dictionary<string, EnemyPool> _enemyPools = new();

        [SerializeField] Transform _enemyArmyHolder;

        public void Initialize()
        {
            // so what I think I should do is create a pool for each enemy type
            // and then when I need to spawn an enemy, I can just grab one from the proper pool
            // and then when the enemy is defeated, I can return it to the pool
            Debug.Log("Initializing enemy pools");

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