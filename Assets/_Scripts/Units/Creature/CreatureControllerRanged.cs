using System.Collections;
using System.Collections.Generic;
using Lis.Battle.Fight;
using Lis.Core.Utilities;
using Lis.Units.Projectile;
using UnityEngine;
using UnityEngine.Serialization;

namespace Lis.Units.Creature
{
    public class CreatureControllerRanged : CreatureController
    {
        [FormerlySerializedAs("_projectileSpawnPoint")] [SerializeField]
        public GameObject ProjectileSpawnPoint;

        ProjectilePoolManager _projectilePoolManager;

        public override void InitializeGameObject()
        {
            base.InitializeGameObject();
            _projectilePoolManager = GetComponent<ProjectilePoolManager>();
        }

        public override void InitializeUnit(Unit unit, int team)
        {
            base.InitializeUnit(unit, team);
            _projectilePoolManager.Initialize(Creature.Projectile);
        }

        protected override IEnumerator PathToOpponent()
        {
            // no obstacle blocking line of sight
            if (HasOpponentInSight())
            {
                yield return base.PathToOpponent();
                yield break;
            }

            Vector3 point = ClosestPositionWithClearLos();
            UnitPathingController.SetStoppingDistance(0);
            yield return UnitPathingController.PathToPosition(point);

            yield return new WaitForSeconds(1f);
            yield return PathToOpponent();
        }

        bool HasOpponentInSight()
        {
            if (Opponent == null) return false;
            //https://answers.unity.com/questions/1164722/raycast-ignore-layers-except.html
            return !Physics.Linecast(transform.position, Opponent.transform.position,
                out _, 1 << Tags.UnpassableLayer);
        }

        public Vector3 ClosestPositionWithClearLos()
        {
            Vector3 dir = transform.position - Opponent.transform.position;
            Dictionary<Vector3, float> distances = new();

            int numberOfLines = 25;
            for (int i = 0; i < numberOfLines; i++)
            {
                Vector3 rotatedLine = Quaternion.AngleAxis(360f * i / numberOfLines, Vector3.up) * dir;
                rotatedLine = rotatedLine.normalized * Creature.AttackRange.GetValue();

                // Debug.DrawLine(_opponent.transform.position, rotatedLine, Color.red, 30f);

                if (!Physics.Linecast(Opponent.transform.position, rotatedLine,
                        out _, 1 << Tags.UnpassableLayer))
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
        Vector3 FindNearestPointOnLine(Vector3 origin, Vector3 end, Vector3 point)
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

            ProjectileController projectileController = _projectilePoolManager.GetObjectFromPool();
            if (Team == 1)
                projectileController = BattleManager.GetComponent<RangedOpponentManager>()
                    .GetProjectileFromPool(Unit.Nature.NatureName);
            projectileController.Initialize(Team);
            projectileController.transform.position = ProjectileSpawnPoint.transform.position;
            if (Opponent == null) yield break;
            Vector3 dir = (Opponent.transform.position - transform.position).normalized;
            dir.y = 0;

            if (Team == 0)
                projectileController.Shoot(this, dir);
            if (Team == 1)
            {
                OpponentProjectileController p = (OpponentProjectileController)projectileController;
                p.Shoot(this, dir, 15, Mathf.FloorToInt(Creature.Power.GetValue()));
            }
        }
    }
}