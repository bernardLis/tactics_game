using UnityEngine;

namespace Lis
{
    public class BattleCreaturePool : PoolManager<BattleCreature>
    {
        public void Initialize(GameObject prefab)
        {
            CreatePool(prefab, 4);
            PoolHolder.parent = transform;
            PoolHolder.localPosition = Vector3.zero;
        }
    }
}