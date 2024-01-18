using System.Collections;
using System.Collections.Generic;



using UnityEngine;

namespace Lis
{
    public class BattleCreatureRanged : BattleCreature
    {
        [SerializeField] protected GameObject _projectileSpawnPoint;

        BattleProjectileManager _battleProjectileManager;

        protected override void InitializeOpponentEntity()
        {
            base.InitializeOpponentEntity();

            _battleProjectileManager = _battleManager.GetComponent<BattleProjectileManager>();
        }

        protected override IEnumerator PathToOpponent()
        {
            // no obstacle blocking line of sight
            if (HasOpponentInSight())
            {
                yield return base.PathToOpponent();
                yield break;
            }

            Vector3 point = ClosestPositionWithClearLOS();
            _agent.stoppingDistance = 0;
            yield return PathToPosition(point);

            yield return new WaitForSeconds(1f);
            yield return PathToOpponent();
        }

        bool HasOpponentInSight()
        {
            if (Opponent == null) return false;
            //https://answers.unity.com/questions/1164722/raycast-ignore-layers-except.html
            return !Physics.Linecast(transform.position, Opponent.transform.position,
                out _, 1 << Tags.BattleObstacleLayer);
        }

        protected Vector3 ClosestPositionWithClearLOS()
        {
            Vector3 dir = transform.position - Opponent.transform.position;
            Dictionary<Vector3, float> distances = new();

            int numberOfLines = 100;
            for (int i = 0; i < numberOfLines; i++)
            {
                Vector3 rotatedLine = Quaternion.AngleAxis(360f * i / numberOfLines, Vector3.up) * dir;
                rotatedLine = rotatedLine.normalized * Creature.AttackRange.GetValue();

                // Debug.DrawLine(_opponent.transform.position, rotatedLine, Color.red, 30f);

                if (!Physics.Linecast(Opponent.transform.position, rotatedLine,
                        out _, 1 << Tags.BattleObstacleLayer))
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
            Vector3 heading = end - origin;
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
            yield return base.Attack();
            if (Team == 1)
            {
                OpponentAttack();
                yield break;
            }

            GameObject projectileInstance = Instantiate(Creature.Projectile, _projectileSpawnPoint.transform.position, Quaternion.identity);
            projectileInstance.transform.parent = _battleManager.EntityHolder;
            BattleProjectile p = projectileInstance.GetComponent<BattleProjectile>();
            p.Initialize(Team);
            Vector3 dir = (Opponent.transform.position - transform.position).normalized;
            p.Shoot(Creature, dir);
        }

        void OpponentAttack()
        {
            BattleProjectileOpponent p = _battleProjectileManager.GetObjectFromPool();
            p.transform.position = transform.position;
            p.Initialize(1);

            Vector3 dir = (Opponent.transform.position - transform.position).normalized;
            dir.y = 0;
            p.Shoot(this, dir, 20, 5);
        }
    }
}
