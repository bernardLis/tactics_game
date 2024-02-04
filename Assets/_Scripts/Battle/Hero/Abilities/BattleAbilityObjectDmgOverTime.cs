using System.Collections;
using System.Collections.Generic;
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
                    StartCoroutine(entity.GetHit(_ability));
                yield return new WaitForSeconds(interval);
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == Tags.BattleObstacleLayer)
                UnpassableHit();

            if (collision.gameObject.TryGetComponent(out BattleBreakableVase bbv))
                bbv.TriggerBreak();

            if (collision.gameObject.TryGetComponent(out BattleEntity battleEntity))
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

        void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out BattleEntity battleEntity))
            {
                if (battleEntity.Team == 0) return; // TODO: hardcoded team number
                RemoveEntityFromList(battleEntity, null);
            }
        }

        void RemoveEntityFromList(BattleEntity entity, EntityFight ignored)
        {
            entity.OnDeath -= RemoveEntityFromList;
            if (EntitiesInCollider.Contains(entity))
                EntitiesInCollider.Remove(entity);
        }
    }
}