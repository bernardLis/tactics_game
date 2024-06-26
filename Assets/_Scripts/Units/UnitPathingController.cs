using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Lis.Units
{
    public class UnitPathingController : MonoBehaviour
    {
        static readonly int AnimMove = Animator.StringToHash("Move");
        NavMeshAgent _agent;
        Animator _animator;
        Vector2Int _avoidancePriorityRange;

        public void Initialize(Vector2Int avoidancePriorityRange)
        {
            _agent = GetComponent<NavMeshAgent>();
            _animator = GetComponentInChildren<Animator>();
            _avoidancePriorityRange = avoidancePriorityRange;
        }

        public void SetAnimator(Animator animator)
        {
            _animator = animator;
        }

        public void SetAvoidancePriorityRange(Vector2Int range)
        {
            _avoidancePriorityRange = range;
        }

        public void SetStoppingDistance(float distance)
        {
            _agent.stoppingDistance = distance;
        }

        public void SetSpeed(float speed)
        {
            _agent.speed = speed;
        }

        public void InitializeUnit(Unit unit)
        {
            _agent.speed = unit.Speed.GetValue();
            unit.Speed.OnValueChanged += i => _agent.speed = i;
            EnableAgent();
        }

        void EnableAgent()
        {
            _agent.enabled = true;
            _agent.isStopped = false;
        }

        public void DisableAgent()
        {
            if (!gameObject.activeSelf) return;
            if (_agent.isActiveAndEnabled) _agent.isStopped = true;
            _agent.enabled = false;
            _animator.SetBool(AnimMove, false);
            _agent.avoidancePriority = 0;
        }

        protected IEnumerator PathToPosition(Vector3 position)
        {
            _agent.enabled = true;
            // TODO: pitiful solution for making entities push each other
            _agent.avoidancePriority = Random.Range(_avoidancePriorityRange.x, _avoidancePriorityRange.y);

            if (!IsAgentOk()) yield break;
            while (!_agent.SetDestination(position)) yield return null;
            while (_agent.pathPending) yield return null;

            _animator.SetBool(AnimMove, true);
        }

        public IEnumerator PathToPositionAndStop(Vector3 position)
        {
            yield return PathToPosition(position);
            while (IsAgentOk() && _agent.remainingDistance > _agent.stoppingDistance)
                yield return new WaitForSeconds(0.1f);

            DisableAgent();
        }

        public virtual IEnumerator PathToTarget(Transform t, float attackRange = 0)
        {
            float dist = attackRange;
            // dist += _agent.radius;
            // if (t.TryGetComponent(out NavMeshAgent navMeshAgent))
            //     dist += navMeshAgent.radius;
            SetStoppingDistance(dist);

            yield return PathToPosition(t.position);
            while (IsAgentOk() && _agent.remainingDistance >= _agent.stoppingDistance)
            {
                if (t == null) yield break;
                _agent.SetDestination(t.position);
                yield return new WaitForSeconds(0.1f);
            }

            DisableAgent();
        }

        bool IsAgentOk()
        {
            if (!_agent.isOnNavMesh) return false;
            if (!_agent.isActiveAndEnabled) return false;
            if (!_agent.enabled) return false;
            return true;
        }
    }
}