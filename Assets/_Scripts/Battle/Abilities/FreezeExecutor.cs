using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeExecutor : AbilityExecutor
{
    protected override IEnumerator ExecuteAbilityCoroutine()
    {
        Debug.Log($"execute freeze on {_entitiesInArea.Count} entities");
        List<GameObject> entityEffects = new();
        foreach (BattleEntity entity in _entitiesInArea)
        {
            entity.StopRunEntityCoroutine();
            Vector3 pos = new Vector3(entity.transform.position.x, 0, entity.transform.position.z);
            GameObject instance = Instantiate(_entityEffectPrefab, pos, Quaternion.identity);
            entityEffects.Add(instance);
        }

        yield return new WaitForSeconds(3f);
        foreach (GameObject g in entityEffects)
        {
            Destroy(g);
        }
        entityEffects.Clear();
        foreach (BattleEntity entity in _entitiesInArea)
        {
            entity.StartRunEntityCoroutine();
        }

        CancelAbility();
    }
}
