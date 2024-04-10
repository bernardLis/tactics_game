using Lis.Core;
using Lis.Units.Boss;
using UnityEngine;

namespace Lis.Battle.Arena
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Arena")]
    public class Arena : BaseScriptableObject
    {
        public GameObject Prefab;
        [Tooltip("How big sphere can fit the area")]
        public int Size;
        public Vector3 PlayerSpawnPoint;
        public Vector3 EnemySpawnPoint;
        public int EnemyPoints;
        public int EnemyPointsGrowth;

        public Boss Boss;

        public void Initialize(int level)
        {
            Boss = Instantiate(GameManager.Instance.UnitDatabase.GetRandomBoss());
            Boss.InitializeBattle(1);

        }
    }
}