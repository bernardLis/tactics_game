using Lis.Core.Utilities;
using UnityEngine;

namespace Lis
{
    public class BattleProjectilePool : PoolManager<BattleProjectile>
    {
        bool _isInitialized;

        public void Initialize(GameObject prefab)
        {
            if (_isInitialized) return;
            _isInitialized = true;

            CreatePool(prefab, 15);
            PoolHolder.parent = BattleManager.Instance.ProjectilePoolHolder;
            PoolHolder.localPosition = Vector3.zero;
        }
    }
}