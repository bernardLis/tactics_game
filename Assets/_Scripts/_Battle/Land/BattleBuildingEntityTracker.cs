using System;
using System.Collections;
using System.Collections.Generic;
using Lis.Units;
using UnityEngine;

namespace Lis
{
    public class BattleBuildingEntityTracker : MonoBehaviour
    {
        public readonly List<UnitController> PlayerEntitiesWithinRange = new();
        public event Action<UnitController> OnEntityEnter;
        public event Action<UnitController> OnEntityExit;

        public void Initialize()
        {
        }

        void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.TryGetComponent(out UnitController battleEntity)) return;
            if (battleEntity.Team == 1) return; // TODO: hardcoded team number
            battleEntity.OnDeath += RemoveEntityFromList;
            PlayerEntitiesWithinRange.Add(battleEntity);
            OnEntityEnter?.Invoke(battleEntity);
        }

        void OnTriggerExit(Collider other)
        {
            if (!other.gameObject.TryGetComponent(out UnitController battleEntity)) return;
            if (battleEntity.Team == 1) return; // TODO: hardcoded team number
            RemoveEntityFromList(battleEntity, null);
            OnEntityExit?.Invoke(battleEntity);
        }

        void RemoveEntityFromList(UnitController entity, UnitController ignored)
        {
            entity.OnDeath -= RemoveEntityFromList;
            if (PlayerEntitiesWithinRange.Contains(entity))
                PlayerEntitiesWithinRange.Remove(entity);
        }
    }
}