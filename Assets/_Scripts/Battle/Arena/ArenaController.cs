using UnityEngine;

namespace Lis.Battle.Arena
{
    public class ArenaController : MonoBehaviour
    {
        [SerializeField] BoxCollider _playerLockerRoom;
        [SerializeField] BoxCollider _enemyLockerRoom;
        [SerializeField] BoxCollider _arena;

        public void Initialize()
        {
        }

        public Vector3 GetRandomPositionInPlayerLockerRoom()
        {
            return RandomPointInBounds(_playerLockerRoom.bounds);
        }

        public Vector3 GetRandomPositionInEnemyLockerRoom()
        {
            return RandomPointInBounds(_enemyLockerRoom.bounds);
        }

        public Vector3 GetRandomPositionInArena()
        {
            return RandomPointInBounds(_arena.bounds);
        }

        public static Vector3 RandomPointInBounds(Bounds bounds)
        {
            return new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z)
            );
        }
    }
}