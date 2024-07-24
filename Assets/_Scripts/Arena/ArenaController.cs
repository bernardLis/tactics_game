using System.Collections.Generic;
using Lis.Core.Utilities;
using UnityEngine;

namespace Lis.Arena
{
    public class ArenaController : Singleton<ArenaController>
    {
        [SerializeField] BoxCollider _playerLockerRoom;
        [SerializeField] BoxCollider _playerBase;
        [SerializeField] BoxCollider _arena;

        [SerializeField] Transform _rewardSpawnPoint;

        [SerializeField] List<BoxCollider> _enemyLockerRooms;
        readonly List<BoxCollider> _activeEnemyLockerRooms = new();


        public void Initialize()
        {
            _activeEnemyLockerRooms.Clear();
            ChooseActiveEnemyLockerRooms(1);
        }

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

        public void ChooseActiveEnemyLockerRooms(int count)
        {
            _activeEnemyLockerRooms.Clear();

            if (count >= _enemyLockerRooms.Count)
            {
                _activeEnemyLockerRooms.AddRange(_enemyLockerRooms);
                return;
            }

            for (int i = 0; i < count; i++)
            {
                BoxCollider lockerRoom = _enemyLockerRooms[Random.Range(0, _enemyLockerRooms.Count)];
                while (_activeEnemyLockerRooms.Contains(lockerRoom))
                    lockerRoom = _enemyLockerRooms[Random.Range(0, _enemyLockerRooms.Count)];
                _activeEnemyLockerRooms.Add(_enemyLockerRooms[Random.Range(0, _enemyLockerRooms.Count)]);
            }
        }

        public Vector3 GetRandomPositionInEnemyLockerRoom()
        {
            return Helpers.RandomPointInBounds(_activeEnemyLockerRooms[Random.Range(0, _activeEnemyLockerRooms.Count)]
                .bounds);
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