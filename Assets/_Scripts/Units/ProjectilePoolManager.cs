using Lis.Battle;
using Lis.Core.Utilities;
using Lis.Units.Projectile;
using UnityEngine;

namespace Lis.Units
{
    public class ProjectilePoolManager : PoolManager<ProjectileController>
    {
        bool _isInitialized;

        public void Initialize(GameObject prefab)
        {
            if (_isInitialized) return;
            _isInitialized = true;

            CreatePool(prefab, 5);
            PoolHolder.parent = BattleManager.Instance.ProjectilePoolHolder;
            PoolHolder.localPosition = Vector3.zero;
        }
    }
}