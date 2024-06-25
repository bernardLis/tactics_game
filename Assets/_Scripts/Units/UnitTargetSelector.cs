using System.Collections.Generic;
using System.Linq;
using Lis.Core;
using UnityEngine;

namespace Lis.Units
{
    public class UnitTargetSelector : MonoBehaviour
    {
        Unit _unit;

        public void Initialize(Unit unit)
        {
            _unit = unit;
        }

        public UnitController ChooseNewTarget(ref List<UnitController> list)
        {
            if (list.Count == 0)
                return null;

            if (_unit.Targeting == TargetingType.Closest)
                return ChooseClosestTarget(ref list);
            if (_unit.Targeting == TargetingType.Random)
                return ChooseRandomTarget(ref list);
            if (_unit.Targeting == TargetingType.Weakest)
                return ChooseWeakestTarget(ref list);
            if (_unit.Targeting == TargetingType.Strongest)
                return ChooseStrongestTarget(ref list);

            return null;
        }

        UnitController ChooseStrongestTarget(ref List<UnitController> list)
        {
            Dictionary<UnitController, float> unitHealth = OrganizeByCurrentHealth(list);
            UnitController strongest = unitHealth.OrderByDescending(pair => pair.Value).First().Key;
            return strongest;
        }

        UnitController ChooseWeakestTarget(ref List<UnitController> list)
        {
            Dictionary<UnitController, float> unitHealth = OrganizeByCurrentHealth(list);
            UnitController weakest = unitHealth.OrderBy(pair => pair.Value).First().Key;
            return weakest;
        }

        Dictionary<UnitController, float> OrganizeByCurrentHealth(List<UnitController> list)
        {
            Dictionary<UnitController, float> unitHealth = new();
            foreach (UnitController uc in list)
            {
                if (uc.IsDead) continue;
                if (unitHealth.ContainsKey(uc)) continue;
                unitHealth.Add(uc, uc.Unit.CurrentHealth.Value);
            }

            return unitHealth;
        }

        UnitController ChooseRandomTarget(ref List<UnitController> list)
        {
            return list[Random.Range(0, list.Count)];
        }

        public UnitController ChooseClosestTarget(ref List<UnitController> list)
        {
            Dictionary<UnitController, float> sqrtDistances = OrganizeByDistance(list);

            var closest = sqrtDistances.OrderByDescending(pair => pair.Value).Reverse().Take(10);

            Dictionary<UnitController, float> closestBiased = new();

            float sum = 0;
            foreach (KeyValuePair<UnitController, float> entry in closest)
            {
                float value = 1 / entry.Value; // 2 / entry.value or 0.1 / entry.value to changed bias
                closestBiased.Add(entry.Key, value);
                sum += value;
            }

            Dictionary<UnitController, float> closestNormalized = new();
            foreach (KeyValuePair<UnitController, float> entry in closestBiased)
                closestNormalized.Add(entry.Key, entry.Value / sum);

            float v = Random.value;
            foreach (KeyValuePair<UnitController, float> entry in closestNormalized)
            {
                if (v < entry.Value)
                    return entry.Key;

                v -= entry.Value;
            }

            return null;

            // Simpler:
            // UnitController closest = sqrtDistances.OrderBy(pair => pair.Value).First().Key;
            // SetOpponent(closest);
        }

        Dictionary<UnitController, float> OrganizeByDistance(List<UnitController> list)
        {
            Dictionary<UnitController, float> sqrtDistances = new();
            foreach (UnitController uc in list)
            {
                if (uc.IsDead) continue;
                if (sqrtDistances.ContainsKey(uc)) continue;
                Vector3 delta = uc.transform.position - transform.position;
                float distance = delta.sqrMagnitude;
                sqrtDistances.Add(uc, distance);
            }

            return sqrtDistances;
        }
    }
}