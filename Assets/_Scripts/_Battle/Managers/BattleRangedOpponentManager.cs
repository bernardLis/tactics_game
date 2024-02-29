using System.Collections.Generic;
using Lis.Core;
using Lis.Core.Utilities;
using UnityEngine;

namespace Lis
{
    public class BattleRangedOpponentManager : PoolManager<BattleProjectileOpponent>
    {
        BattleManager _battleManager;

        [SerializeField] Transform _rangedOpponentHolder;

        [SerializeField] GameObject _projectilePrefab;

        [SerializeField] GameObject _projectileBiggerWithTimePrefab;
        [SerializeField] GameObject _projectileQuickPrefab;
        [SerializeField] GameObject _projectileWallBouncePrefab;

        [SerializeField] GameObject _rangedOpponentPrefab;

        readonly List<GameObject> _projectilePoolBiggerWithTime = new();
        readonly List<GameObject> _projectilePoolQuick = new();
        readonly List<GameObject> _projectilePoolWallBounce = new();
        readonly List<GameObject> _rangedOpponentPool = new();

        public void Initialize()
        {
            _battleManager = GetComponent<BattleManager>();

            CreatePool(_projectilePrefab);

            CreateNewPool(_projectileBiggerWithTimePrefab, _projectilePoolBiggerWithTime);
            CreateNewPool(_projectileQuickPrefab, _projectilePoolQuick);
            CreateNewPool(_projectileWallBouncePrefab, _projectilePoolWallBounce);

            CreateNewPool(_rangedOpponentPrefab, _rangedOpponentPool);
        }

        void CreateNewPool(GameObject prefab, List<GameObject> pool)
        {
            for (int i = 0; i < 25; i++)
            {
                GameObject obj = Instantiate(prefab, _rangedOpponentHolder);
                obj.SetActive(false);
                pool.Add(obj);
            }
        }

        GameObject GetFromPool(List<GameObject> pool)
        {
            for (int i = 0; i < pool.Count; i++)
            {
                if (pool[i].activeSelf) continue;
                return pool[i];
            }

            return null;
        }

        public void SpawnRangedOpponent(Entity entity, Vector3 pos)
        {
            entity.InitializeBattle(1);
            
            GameObject rangedOpponent = GetFromPool(_rangedOpponentPool);
            if (rangedOpponent == null) return;

            rangedOpponent.transform.position = pos;
            rangedOpponent.SetActive(true);

            BattleRangedOpponent opponent = rangedOpponent.GetComponent<BattleRangedOpponent>();
            opponent.InitializeEntity(entity, 1);
            _battleManager.AddOpponentArmyEntity(opponent);
        }

        public BattleProjectileOpponent GetProjectileFromPool(ElementName element)
        {
            BattleProjectileOpponent projectile = GetObjectFromPool();

            switch (element)
            {
                case ElementName.Earth:
                    projectile = GetFromPool(_projectilePoolBiggerWithTime).GetComponent<BattleProjectileOpponent>();
                    break;
                case ElementName.Fire:
                    projectile = GetFromPool(_projectilePoolQuick).GetComponent<BattleProjectileOpponent>();
                    break;
                case ElementName.Wind:
                    projectile = GetFromPool(_projectilePoolWallBounce).GetComponent<BattleProjectileOpponent>();
                    break;
            }

            return projectile;
        }
    }
}