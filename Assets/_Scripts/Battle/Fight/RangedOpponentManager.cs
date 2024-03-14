using System.Collections.Generic;
using Lis.Battle.Tiles.Building;
using Lis.Core;
using Lis.Units;
using Lis.Units.Minion;
using Lis.Units.Projectile;
using UnityEngine;

namespace Lis.Battle.Fight
{
    public class RangedOpponentManager : MonoBehaviour
    {
        BattleManager _battleManager;

        [SerializeField] Transform _rangedOpponentHolder;

        [SerializeField] GameObject _projectilePrefab;
        [SerializeField] GameObject _projectileBiggerWithTimePrefab;
        [SerializeField] GameObject _projectileQuickPrefab;
        [SerializeField] GameObject _projectileWallBouncePrefab;

        [SerializeField] GameObject _rangedOpponentPrefab;

        readonly List<GameObject> _projectilePool = new();
        readonly List<GameObject> _projectilePoolBiggerWithTime = new();
        readonly List<GameObject> _projectilePoolQuick = new();
        readonly List<GameObject> _projectilePoolWallBounce = new();
        readonly List<GameObject> _rangedOpponentPool = new();

        public void Initialize()
        {
            _battleManager = GetComponent<BattleManager>();

            CreateNewPool(_projectilePrefab, _projectilePool);
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
                if (obj.TryGetComponent(out UnitController unit))
                    unit.InitializeGameObject();
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

            RangedMinionController controller
                = rangedOpponent.GetComponent<RangedMinionController>();
            controller.InitializeUnit(unit, 1);
            controller.InitializeHostileCreature(default);
            _battleManager.AddOpponentArmyEntity(controller);
        }

        public OpponentProjectileController GetProjectileFromPool(NatureName nature)
        {
            OpponentProjectileController opponentProjectileController = GetFromPool(_projectilePool)
                .GetComponent<OpponentProjectileController>();

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