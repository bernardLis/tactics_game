using System.Collections;
using System.Collections.Generic;
using Lis.Battle.Pickup;
using Lis.Core.Utilities;
using UnityEngine;

namespace Lis.Units.Hero.Ability
{
    public class ObjectControllerDmgOverTime : ObjectController
    {
        protected readonly List<UnitController> UnitsInCollider = new();

        public override void Execute(Vector3 pos, Quaternion q)
        {
            UnitsInCollider.Clear();
            base.Execute(pos, q);
        }

        protected virtual IEnumerator DamageCoroutine(float endTime, float interval = 0.5f)
        {
            while (Time.time < endTime)
            {
                List<UnitController> currentEntities = new(UnitsInCollider);
                foreach (UnitController entity in currentEntities)
                    StartCoroutine(entity.GetHit(Ability));
                yield return new WaitForSeconds(interval);
            }
        }

        void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.layer == Tags.UnpassableLayer)
                UnpassableHit();

            if (col.gameObject.TryGetComponent(out BreakableVaseController bbv))
                bbv.TriggerBreak();

            if (col.gameObject.TryGetComponent(out UnitController battleEntity))
            {
                if (battleEntity.Team == 0) return; // TODO: hardcoded team number
                battleEntity.OnDeath += RemoveEntityFromList;
                UnitsInCollider.Add(battleEntity);
            }
        }

        protected virtual void UnpassableHit()
        {
            // meant to be overridden
        }

        void OnTriggerExit(Collider col)
        {
            if (col.gameObject.TryGetComponent(out UnitController battleEntity))
            {
                if (battleEntity.Team == 0) return; // TODO: hardcoded team number
                RemoveEntityFromList(battleEntity, null);
            }
        }

        void RemoveEntityFromList(UnitController entity, UnitController ignored)
        {
            entity.OnDeath -= RemoveEntityFromList;
            if (UnitsInCollider.Contains(entity))
                UnitsInCollider.Remove(entity);
        }
    }
}