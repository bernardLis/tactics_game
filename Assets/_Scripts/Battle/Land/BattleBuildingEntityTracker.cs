using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lis
{
    public class BattleBuildingEntityTracker : MonoBehaviour
    {
        public readonly List<BattleEntity> PlayerEntitiesWithinRange = new();
        public event Action<BattleEntity> OnEntityInRange;

        public void Initialize()
        {
        }

        void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.TryGetComponent(out BattleEntity battleEntity)) return;
            if (battleEntity.Team == 1) return; // TODO: hardcoded team number
            battleEntity.OnDeath += RemoveEntityFromList;
            PlayerEntitiesWithinRange.Add(battleEntity);
            OnEntityInRange?.Invoke(battleEntity);
        }

        void OnTriggerExit(Collider other)
        {
            if (!other.gameObject.TryGetComponent(out BattleEntity battleEntity)) return;
            if (battleEntity.Team == 1) return; // TODO: hardcoded team number
            RemoveEntityFromList(battleEntity, null);
        }

        void RemoveEntityFromList(BattleEntity entity, BattleEntity ignored)
        {
            entity.OnDeath -= RemoveEntityFromList;
            if (PlayerEntitiesWithinRange.Contains(entity))
                PlayerEntitiesWithinRange.Remove(entity);
        }
    }
}