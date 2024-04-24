using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Lis.Battle.Arena
{
    public class ArenaManager : MonoBehaviour
    {
        GameObject _arenaObject;
        Arena _arena;
        ArenaController _arenaController;

        public void Initialize(Battle battle)
        {
            _arena = battle.CurrentArena;
            _arenaObject = Instantiate(_arena.Prefab, Vector3.zero, Quaternion.identity);
            _arenaController = _arenaObject.GetComponent<ArenaController>();
        }

        public Vector3 GetRandomPositionInPlayerLockerRoom()
        {
            return _arenaController.GetRandomPositionInPlayerLockerRoom();
        }

        public Vector3 GetRandomPositionInEnemyLockerRoom()
        {
            return _arenaController.GetRandomPositionInEnemyLockerRoom();
        }

        public Vector3 GetRandomPositionInArena()
        {
            return _arenaController.GetRandomPositionInArena();
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