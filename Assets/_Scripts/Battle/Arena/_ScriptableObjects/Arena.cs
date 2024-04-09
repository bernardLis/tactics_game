using Lis.Core;
using UnityEngine;

namespace Lis
{
    [CreateAssetMenu(menuName = "ScriptableObject/Battle/Arena")]
    public class Arena : BaseScriptableObject
    {
        public GameObject Prefab;
        public Vector3 PlayerSpawnPoint;
        public Vector3 EnemySpawnPoint;
        public int EnemyPoints;
        public int EnemyPointsGrowth;
    }
}
