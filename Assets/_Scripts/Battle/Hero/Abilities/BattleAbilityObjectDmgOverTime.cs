using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAbilityObjectDmgOverTime : BattleAbilityObject
{
    List<BattleEntity> _entitiesInCollider = new();

    protected IEnumerator DamageCoroutine(float endTime, float interval = 0.5f)
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
        if (_entitiesInCollider.Contains(entity))
            _entitiesInCollider.Remove(entity);
    }

}
