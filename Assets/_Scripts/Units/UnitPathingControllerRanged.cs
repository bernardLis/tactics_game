using System.Collections;
using System.Collections.Generic;
using Lis.Core.Utilities;
using UnityEngine;

namespace Lis.Units
{
    public class UnitPathingControllerRanged : UnitPathingController
    {
        Transform _target;
        float _attackRange;

        public override IEnumerator PathToTarget(Transform t, float attackRange = 0f)
        {
            _target = t;
            _attackRange = attackRange;

            if (HasTargetInSight())
            {
                yield return base.PathToTarget(t, attackRange);
                yield break;
            }

            Vector3 point = ClosestPositionWithClearLos();
            SetStoppingDistance(0);
            yield return PathToPosition(point);

            yield return new WaitForSeconds(1f);
            yield return PathToTarget(t, attackRange);
        }

        bool HasTargetInSight()
        {
            if (_target == null) return false;
            //https://answers.unity.com/questions/1164722/raycast-ignore-layers-except.html
            return !Physics.Linecast(transform.position, _target.transform.position,
                out _, 1 << Tags.UnpassableLayer);
        }

        public Vector3 ClosestPositionWithClearLos()
        {
            Vector3 dir = transform.position - _target.transform.position;
            Dictionary<Vector3, float> distances = new();

            int numberOfLines = 25;
            for (int i = 0; i < numberOfLines; i++)
            {
                Vector3 rotatedLine = Quaternion.AngleAxis(360f * i / numberOfLines, Vector3.up) * dir;
                rotatedLine = rotatedLine.normalized * _attackRange;

                // Debug.DrawLine(_opponent.transform.position, rotatedLine, Color.red, 30f);

                if (!Physics.Linecast(_target.transform.position, rotatedLine,
                        out _, 1 << Tags.UnpassableLayer))
                {
                    //   Debug.DrawLine(_opponent.transform.position, rotatedLine, Color.blue, 30f);

                    Vector3 point = FindNearestPointOnLine(_target.transform.position,
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
    }
}