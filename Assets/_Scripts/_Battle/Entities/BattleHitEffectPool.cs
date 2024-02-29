using Lis.Core.Utilities;
using UnityEngine;

namespace Lis
{
    public class BattleHitEffectPool : PoolManager<BattleHitEffect>
    {
        bool _isInitialized;

        public void Initialize(GameObject prefab)
        {
            if (_isInitialized) return;
            _isInitialized = true;

            CreatePool(prefab, 5);
            PoolHolder.parent = transform;
            PoolHolder.localPosition = Vector3.zero;
        }
    }
}