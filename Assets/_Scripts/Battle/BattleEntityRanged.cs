using System.Collections;
using UnityEngine;
using DG.Tweening;

public class BattleEntityRanged : BattleEntity
{
    [SerializeField] GameObject _projectileSpawnPoint;

    protected override IEnumerator PathToTarget()
    {
        // no obstacle blocking line of sight
        if (!Physics.Linecast(transform.position, _opponent.transform.position,
                out RaycastHit hit, 1 << Tags.BattleObstacleLayer)) //https://answers.unity.com/questions/1164722/raycast-ignore-layers-except.html
        {
            _agent.stoppingDistance = ArmyEntity.AttackRange;
            yield return base.PathToTarget();
            yield break;
        }

        Vector3 point = NearestPointOnLine(_opponent.transform.position,
                ClearLineOfSightToOpponent(), transform.position);

        // path to that point
        _agent.stoppingDistance = 0;
        _agent.enabled = true;
        while (!_agent.SetDestination(point)) yield return null;
        _animator.SetBool("Move", true);
        while (_agent.pathPending) yield return null;

        // path to target
        while (_agent.enabled && _agent.remainingDistance > 0.01f)
        {
            _agent.SetDestination(NearestPointOnLine(_opponent.transform.position,
                    ClearLineOfSightToOpponent(), transform.position));
            yield return new WaitForSeconds(0.1f);
        }
        // reached destination
        _animator.SetBool("Move", false);
        _agent.enabled = false;

        yield return null;
    }

    Vector3 ClearLineOfSightToOpponent()
    {
        Vector3 dir = transform.position - _opponent.transform.position;
        Vector3 dirNormalized = dir.normalized;
        Vector3 dirNormalizedKindaReversed = new Vector3(dirNormalized.z, 0, dirNormalized.x);

        for (int i = 0; i < 10; i++)
        {
            Vector3 dir1 = dir + dirNormalizedKindaReversed * i * 4;
            Vector3 dir2 = dir - dirNormalizedKindaReversed * i * 4;

            //    Debug.DrawRay(_opponent.transform.position, dir1, Color.blue, 0.5f);
            //      Debug.DrawRay(_opponent.transform.position, dir2, Color.green, 0.5f);

            if (!Physics.Linecast(_opponent.transform.position, dir1 * 40,
                out RaycastHit newHit, 1 << Tags.BattleObstacleLayer))
            {
                //       Debug.DrawRay(_opponent.transform.position, dir1, Color.red, 2f);
                return dir1;
            }

            if (!Physics.Linecast(_opponent.transform.position, dir2 * 40,
                out RaycastHit newHit2, 1 << Tags.BattleObstacleLayer))
            {
                //       Debug.DrawRay(_opponent.transform.position, dir2, Color.red, 2f);
                return dir2;
            }
        }
        return Vector3.zero;
    }


    //https://forum.unity.com/threads/how-do-i-find-the-closest-point-on-a-line.340058/
    Vector3 NearestPointOnLine(Vector3 linePnt, Vector3 lineDir, Vector3 pnt)
    {
        lineDir.Normalize();//this needs to be a unit vector
        var v = pnt - linePnt;
        var d = Vector3.Dot(v, lineDir);
        return linePnt + lineDir * d;
    }

    protected override IEnumerator Attack()
    {
        while (!CanAttack()) yield return null;
        if (!IsOpponentInRange()) StartRunEntityCoroutine();

        yield return transform.DODynamicLookAt(_opponent.transform.position, 0.2f).WaitForCompletion();
        _animator.SetTrigger("Attack");

        // HERE: projectile spawning and animation delay per entity
        yield return new WaitWhile(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.5f);
        GameObject projectileInstance = Instantiate(ArmyEntity.Projectile, _projectileSpawnPoint.transform.position, Quaternion.identity);
        projectileInstance.transform.parent = _GFX.transform;

        // HERE: projectile speed
        projectileInstance.GetComponent<Projectile>().Shoot(this, _opponent, 20, ArmyEntity.Power);

        yield return base.Attack();
    }

}
