using System.Collections.Generic;
using Lis.Core;
using Lis.Core.Utilities;
using Lis.Units;
using Lis.Units.Minion;
using Lis.Units.Projectile;
using UnityEngine;

namespace Lis.Battle.Fight
{
    public class RangedOpponentManager : PoolManager<OpponentProjectileController>
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

        public void SpawnRangedOpponent(Unit unit, Vector3 pos)
        {
            unit.InitializeBattle(1);

            GameObject rangedOpponent = GetFromPool(_rangedOpponentPool);
            if (rangedOpponent == null) return;

            rangedOpponent.transform.position = pos;
            rangedOpponent.SetActive(true);

            RangedMinionController minionController = rangedOpponent.GetComponent<RangedMinionController>();
            minionController.InitializeUnit(unit, 1);
            _battleManager.AddOpponentArmyEntity(minionController);
        }

        public OpponentProjectileController GetProjectileFromPool(NatureName nature)
        {
            OpponentProjectileController opponentProjectileController = GetObjectFromPool();

            switch (nature)
            {
                case NatureName.Earth:
                    opponentProjectileController = GetFromPool(_projectilePoolBiggerWithTime)
                        .GetComponent<OpponentProjectileController>();
                    break;
                case NatureName.Fire:
                    opponentProjectileController =
                        GetFromPool(_projectilePoolQuick).GetComponent<OpponentProjectileController>();
                    break;
                case NatureName.Wind:
                    opponentProjectileController = GetFromPool(_projectilePoolWallBounce)
                        .GetComponent<OpponentProjectileController>();
                    break;
            }

            return opponentProjectileController;
        }
    }
}