using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeExecutor : AbilityExecutor
{
    protected override IEnumerator ExecuteAbilityCoroutine()
    {
        Debug.Log($"Executing freeze on {_entitiesInArea.Count} entities");
        List<GameObject> entityEffects = new();
        foreach (BattleEntity entity in _entitiesInArea)
        {
            entity.StopRunEntityCoroutine();
            entity.GetComponentInChildren<Animator>().enabled = false;
            Vector3 pos = new Vector3(entity.transform.position.x, 0, entity.transform.position.z);
            GameObject instance = Instantiate(_entityEffectPrefab, pos, Quaternion.identity);
            entityEffects.Add(instance);

            instance.GetComponent<FreezeEntityEffect>().SetDelays(_selectedAbility.GetPower());
        }


        yield return new WaitForSeconds(_selectedAbility.GetPower() + 0.2f);
        foreach (BattleEntity entity in _entitiesInArea)
        {
            entity.GetComponentInChildren<Animator>().enabled = true;
            entity.StartRunEntityCoroutine();
        }

        yield return new WaitForSeconds(7f);
        foreach (GameObject g in entityEffects)
            Destroy(g);
        entityEffects.Clear();

        CancelAbility();
    }
}
