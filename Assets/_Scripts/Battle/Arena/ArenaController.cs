using Lis.Core.Utilities;
using UnityEngine;

namespace Lis.Battle.Arena
{
    public class ArenaController : Singleton<ArenaController>
    {
        [SerializeField] BoxCollider _playerLockerRoom;
        [SerializeField] BoxCollider _enemyLockerRoom;
        [SerializeField] BoxCollider _arena;

        public void Initialize()
        {
        }

        public bool IsPositionInPlayerLockerRoom(Vector3 pos)
        {
            pos.y = _playerLockerRoom.bounds.min.y; // ignoring y axis
            return _playerLockerRoom.bounds.Contains(pos);
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
    }
}