using System.Collections.Generic;
using Lis.Core;
using Lis.Units;
using Lis.Units.Projectile;
using UnityEngine;

namespace Lis.Battle.Fight
{
    public class RangedOpponentManager : MonoBehaviour
    {
        [SerializeField] private Transform _rangedOpponentHolder;

        [SerializeField] private GameObject _projectilePrefab;
        [SerializeField] private GameObject _projectileBiggerWithTimePrefab;
        [SerializeField] private GameObject _projectileQuickPrefab;
        [SerializeField] private GameObject _projectileWallBouncePrefab;

        [SerializeField] private GameObject _rangedOpponentPrefab;

        private readonly List<GameObject> _projectilePool = new();
        private readonly List<GameObject> _projectilePoolBiggerWithTime = new();
        private readonly List<GameObject> _projectilePoolQuick = new();
        private readonly List<GameObject> _projectilePoolWallBounce = new();

        public void Initialize()
        {
            CreateNewPool(_projectilePrefab, _projectilePool);
            CreateNewPool(_projectileBiggerWithTimePrefab, _projectilePoolBiggerWithTime);
            CreateNewPool(_projectileQuickPrefab, _projectilePoolQuick);
            CreateNewPool(_projectileWallBouncePrefab, _projectilePoolWallBounce);
        }

        private void CreateNewPool(GameObject prefab, List<GameObject> pool)
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

        private GameObject GetFromPool(List<GameObject> pool)
        {
            for (int i = 0; i < pool.Count; i++)
            {
                if (pool[i].activeSelf) continue;
                return pool[i];
            }

            GameObject obj = Instantiate(pool[0], _rangedOpponentHolder);
            obj.SetActive(false);
            pool.Add(obj);
            if (obj.TryGetComponent(out UnitController unit))
                unit.InitializeGameObject();

            return obj;
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