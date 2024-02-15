using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Lis
{
    public class BattleEntityPathing : MonoBehaviour
    {
        NavMeshAgent _agent;
        Vector2Int _avoidancePriorityRange;
        Animator _animator;

        static readonly int AnimMove = Animator.StringToHash("Move");

        public void Initialize(Vector2Int avoidancePriorityRange)
        {
            _agent = GetComponent<NavMeshAgent>();
            _animator = GetComponentInChildren<Animator>();
            _avoidancePriorityRange = avoidancePriorityRange;
        }

        public void SetAvoidancePriorityRange(Vector2Int range) => _avoidancePriorityRange = range;
        public void SetStoppingDistance(float distance) => _agent.stoppingDistance = distance;
        public void SetSpeed(float speed) => _agent.speed = speed;

        public void InitializeEntity(EntityMovement em)
        {
            _agent.speed = em.Speed.GetValue();
            em.Speed.OnValueChanged += (i) => _agent.speed = i;
        }

        public void EnableAgent()
        {
            _agent.enabled = true;
            _agent.isStopped = false;
        }

        public void DisableAgent()
        {
            if (_agent.isActiveAndEnabled) _agent.isStopped = true;
            _agent.enabled = false;
            _animator.SetBool(AnimMove, false);
            _agent.avoidancePriority = 0;
        }

        public IEnumerator PathToPosition(Vector3 position)
        {
            // EntityLog.Add($"{BattleManager.GetTime()}: Path to position is called {position}");

            _agent.enabled = true;
            // TODO: pitiful solution for making entities push each other
            _agent.avoidancePriority = Random.Range(_avoidancePriorityRange.x, _avoidancePriorityRange.y);

            while (!_agent.SetDestination(position)) yield return null;
            while (_agent.pathPending) yield return null;

            _animator.SetBool(AnimMove, true);
        }

        public IEnumerator PathToPositionAndStop(Vector3 position)
        {
            yield return PathToPosition(position);
            while (_agent.enabled && _agent.remainingDistance > _agent.stoppingDistance)
                yield return new WaitForSeconds(0.1f);

            DisableAgent();
        }

        public IEnumerator PathToTarget(Transform t)
        {
            // EntityLog.Add($"{BattleManager.GetTime()}: Path to target is called {t}");

            yield return PathToPosition(t.position);
            while (_agent.enabled && _agent.remainingDistance > _agent.stoppingDistance)
            {
                if (t == null) yield break;
                _agent.SetDestination(t.position);
                yield return new WaitForSeconds(0.1f);
            }

            DisableAgent();
        }
    }
}