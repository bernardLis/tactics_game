using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleCreatureRanged : BattleCreature
{
    [SerializeField] protected GameObject _projectileSpawnPoint;

    protected override IEnumerator PathToTarget()
    {
        // no obstacle blocking line of sight
        if (HasOpponentInSight())
        {
            _agent.stoppingDistance = Creature.AttackRange;
            yield return base.PathToTarget();
            yield break;
        }

        if (_hasSpecialMove && CanUseSpecialAbility())
        {
            yield return SpecialAbility();
            StartPathToTargetCoroutine();
            yield break;
        }

        Vector3 point = ClosesPositionWithClearLOS();

        // path to that point
        _agent.avoidancePriority = Random.Range(1, 100);
        _agent.stoppingDistance = 0;
        _agent.enabled = true;
        while (!_agent.SetDestination(point)) yield return null;
        Animator.SetBool("Move", true);
        while (_agent.pathPending) yield return null;

        // path to target
        while (_agent.enabled && _agent.remainingDistance > 0.01f)
        {
            yield return new WaitForSeconds(0.1f);
            if (!HasOpponentInSight())
            {
                yield return StartPathToTargetCoroutine();
                yield break;
            }
            // yield return PathToTarget();// <- could be used to refresh path if target moved
        }

        // reached destination
        Animator.SetBool("Move", false);
        _agent.enabled = false;
        yield return StartPathToTargetCoroutine();
    }

    bool HasOpponentInSight()
    {
        //https://answers.unity.com/questions/1164722/raycast-ignore-layers-except.html
        return !Physics.Linecast(transform.position, Opponent.transform.position,
                    out RaycastHit hit, 1 << Tags.BattleObstacleLayer);

    }

    protected Vector3 ClosesPositionWithClearLOS()
    {
        Vector3 dir = transform.position - Opponent.transform.position;
        Dictionary<Vector3, float> distances = new();

        int numberOfLines = 100;
        for (int i = 0; i < numberOfLines; i++)
        {
            Vector3 rotatedLine = Quaternion.AngleAxis(360f * i / numberOfLines, Vector3.up) * dir;
            rotatedLine = rotatedLine.normalized * Creature.AttackRange;

            // Debug.DrawLine(_opponent.transform.position, rotatedLine, Color.red, 30f);

            if (!Physics.Linecast(Opponent.transform.position, rotatedLine,
                    out RaycastHit newHit, 1 << Tags.BattleObstacleLayer))
            {
                //   Debug.DrawLine(_opponent.transform.position, rotatedLine, Color.blue, 30f);

                Vector3 point = FindNearestPointOnLine(Opponent.transform.position,
                        rotatedLine, transform.position);
                if (!distances.ContainsKey(point))
                    distances.Add(point, Vector3.Distance(transform.position, point));
            }
        }

        float minDistance = float.MaxValue;
        Vector3 closestPoint = Vector3.zero;
        foreach (KeyValuePair<Vector3, float> kvp in distances)
        {
            if (kvp.Value < minDistance)
            {
                minDistance = kvp.Value;
                closestPoint = kvp.Key;
            }
        }
        return closestPoint;
    }

    // https://stackoverflow.com/questions/51905268/how-to-find-closest-point-on-line
    public Vector3 FindNearestPointOnLine(Vector3 origin, Vector3 end, Vector3 point)
    {
        //Get heading
        Vector3 heading = (end - origin);
        float magnitudeMax = heading.magnitude;
        heading.Normalize();

        //Do projection from the point but clamp it
        Vector3 lhs = point - origin;
        float dotP = Vector3.Dot(lhs, heading);
        dotP = Mathf.Clamp(dotP, 0f, magnitudeMax);
        return origin + heading * dotP;
    }

    protected override IEnumerator Attack()
    {
        while (!CanAttack()) yield return null;

        if (_hasSpecialAttack & CanUseSpecialAbility())
        {
            yield return SpecialAbility();
            yield return base.Attack();
            yield break;
        }

        if (!IsOpponentInRange())
        {
            StartRunEntityCoroutine();
            yield break;
        }

        yield return transform.DODynamicLookAt(Opponent.transform.position, 0.2f).WaitForCompletion();
        Animator.SetTrigger("Attack");

        yield return new WaitWhile(() => Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.5f);
        GameObject projectileInstance = Instantiate(Creature.Projectile, _projectileSpawnPoint.transform.position, Quaternion.identity);
        projectileInstance.transform.parent = _GFX.transform;

        projectileInstance.GetComponent<Projectile>().Shoot(this, Opponent, Creature.GetPower());

        yield return base.Attack();
    }

}
