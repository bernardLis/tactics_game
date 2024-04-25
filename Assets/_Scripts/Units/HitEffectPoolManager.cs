using Lis.Core.Utilities;
using UnityEngine;

namespace Lis.Units
{
    public class HitEffectPoolManager : PoolManager<HitEffectController>
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

        public void ResetPool()
        {
            foreach (var hitEffect in Pool)
            {
                if (hitEffect == null) continue;
                hitEffect.transform.parent = PoolHolder.transform;
            }
        }
    }
}