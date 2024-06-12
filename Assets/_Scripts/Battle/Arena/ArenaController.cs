using Lis.Battle.Fight;
using Lis.Core.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Lis.Battle.Arena
{
    public class ArenaController : Singleton<ArenaController>
    {
        [SerializeField] BoxCollider _playerLockerRoom;
        [SerializeField] BoxCollider _playerBase;
        [SerializeField] BoxCollider _enemyLockerRoom;
        [SerializeField] BoxCollider _arena;

        [SerializeField] Transform _rewardSpawnPoint;

        public bool IsPositionInPlayerLockerRoom(Vector3 pos)
        {
            pos.y = _playerLockerRoom.bounds.min.y; // ignoring y axis
            return _playerLockerRoom.bounds.Contains(pos);
        }

        public bool IsPositionInPlayerBase(Vector3 pos)
        {
            pos.y = _playerBase.bounds.min.y; // ignoring y axis
            return _playerBase.bounds.Contains(pos);
        }

        public bool IsPositionInArena(Vector3 pos)
        {
            pos.y = _arena.bounds.min.y; // ignoring y axis
            return _arena.bounds.Contains(pos);
        }

        public Vector3 GetRandomPositionInPlayerLockerRoom()
        {
            return Helpers.RandomPointInBounds(_playerLockerRoom.bounds);
        }

        public Vector3 GetRandomPositionInEnemyLockerRoom()
        {
            return Helpers.RandomPointInBounds(_enemyLockerRoom.bounds);
        }

        public Vector3 GetRandomPositionInArena()
        {
            return Helpers.RandomPointInBounds(_arena.bounds);
        }

        public Vector3 GetRewardSpawnPoint()
        {
            return _rewardSpawnPoint.position;
        }
    }
}