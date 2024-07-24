using System.Collections.Generic;
using Lis.Core.Utilities;
using UnityEngine;
using UnityEngine.AI;

namespace Lis.Arena
{
    public class ArenaManager : Singleton<ArenaManager>
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
            SetActiveEnemyLockerRooms(1);
        }


        public bool IsPositionInArena(Vector3 pos)
        {
            pos.y = _arena.bounds.min.y; // ignoring y axis
            return _arena.bounds.Contains(pos);
        }

        public void SetActiveEnemyLockerRooms(int count)
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

        // TODO: need to make sure it is within arena bounds
        public Vector3 GetRandomPositionWithinRange(Vector3 center, float range)
        {
            int tries = 0;
            while (tries < 100)
            {
                tries++;
                Vector3 randomPoint = center + Random.insideUnitSphere * range;
                randomPoint.y = 0;

                if (IsPositionOnNavmesh(randomPoint))
                    return randomPoint;
            }

            Debug.LogError($"Could not find random position within range {range} of {center} on active tile");
            return Vector3.zero;
        }

        public Vector3 GetRandomPositionInRange(Vector3 center, float range)
        {
            int tries = 0;
            while (tries < 100)
            {
                tries++;
                range -= 0.1f;
                Vector2 r = Random.insideUnitCircle * range;
                Vector3 randomPoint = center + new Vector3(r.x, 0, r.y);
                randomPoint.y = 0;
                if (IsPositionOnNavmesh(randomPoint))
                    return randomPoint;
            }

            Debug.LogError($"Could not find random position within range {range} on active tile");
            return Vector3.zero;
        }

        bool IsPositionOnNavmesh(Vector3 pos)
        {
            return NavMesh.SamplePosition(pos, out _, 0.3f, NavMesh.AllAreas);
        }
    }
}