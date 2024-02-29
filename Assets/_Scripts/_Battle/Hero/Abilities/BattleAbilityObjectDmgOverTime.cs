using System.Collections;
using System.Collections.Generic;
using Lis.Core.Utilities;
using UnityEngine;

namespace Lis
{
    public class BattleAbilityObjectDmgOverTime : BattleAbilityObject
    {
        protected readonly List<BattleEntity> EntitiesInCollider = new();

        public override void Execute(Vector3 pos, Quaternion q)
        {
            EntitiesInCollider.Clear();
            base.Execute(pos, q);
        }

        protected virtual IEnumerator DamageCoroutine(float endTime, float interval = 0.5f)
        {
            while (Time.time < endTime)
            {
                List<BattleEntity> currentEntities = new(EntitiesInCollider);
                foreach (BattleEntity entity in currentEntities)
                    StartCoroutine(entity.GetHit(Ability));
                yield return new WaitForSeconds(interval);
            }
        }

        void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.layer == Tags.UnpassableLayer)
                UnpassableHit();

            if (col.gameObject.TryGetComponent(out BattleBreakableVase bbv))
                bbv.TriggerBreak();

            if (col.gameObject.TryGetComponent(out BattleEntity battleEntity))
            {
                if (battleEntity.Team == 0) return; // TODO: hardcoded team number
                battleEntity.OnDeath += RemoveEntityFromList;
                EntitiesInCollider.Add(battleEntity);
            }
        }

        protected virtual void UnpassableHit()
        {
            // meant to be overridden
        }

        void OnTriggerExit(Collider col)
        {
            if (col.gameObject.TryGetComponent(out BattleEntity battleEntity))
            {
                if (battleEntity.Team == 0) return; // TODO: hardcoded team number
                RemoveEntityFromList(battleEntity, null);
            }
        }

        void RemoveEntityFromList(BattleEntity entity, BattleEntity ignored)
        {
            entity.OnDeath -= RemoveEntityFromList;
            if (EntitiesInCollider.Contains(entity))
                EntitiesInCollider.Remove(entity);
        }
    }
}