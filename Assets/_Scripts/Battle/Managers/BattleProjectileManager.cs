

using UnityEngine;

namespace Lis
{
    public class BattleProjectileManager : PoolManager<BattleProjectileOpponent>
    {
        [SerializeField] GameObject _projectilePrefab;

        public void Initialize()
        {
            CreatePool(_projectilePrefab);
        }

    }
}
