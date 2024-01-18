using System.Collections;
using System.Collections.Generic;


using UnityEngine;

namespace Lis
{
    public class BattleAbilityObjectDmgOverTime : BattleAbilityObject
    {
        protected List<BattleEntity> _entitiesInCollider = new();

        public override void Execute(Vector3 pos, Quaternion q)
        {
            _entitiesInCollider.Clear();
            base.Execute(pos, q);
        }

        protected virtual IEnumerator DamageCoroutine(float endTime, float interval = 0.5f)
        {
            while (Time.time < endTime)
            {
                List<BattleEntity> currentEntities = new(_entitiesInCollider);
                foreach (BattleEntity entity in currentEntities)
                    StartCoroutine(entity.GetHit(_ability));
                yield return new WaitForSeconds(interval);
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out BattleBreakableVase bbv))
                bbv.TriggerBreak();

            if (collision.gameObject.TryGetComponent(out BattleEntity battleEntity))
            {
                if (battleEntity.Team == 0) return; // TODO: hardcoded team number
                battleEntity.OnDeath += RemoveEntityFromList;
                _entitiesInCollider.Add(battleEntity);
            }
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
            if (_entitiesInCollider.Contains(entity))
                _entitiesInCollider.Remove(entity);
        }
    }
}
