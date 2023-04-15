using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealExecutor : AbilityExecutor
{
    protected override IEnumerator ExecuteAbilityCoroutine()
    {
        foreach (BattleEntity entity in _entitiesInArea)
        {
            entity.GetHealed(_selectedAbility);
        }

        yield return new WaitForSeconds(3f);
        CancelAbility();
    }

}
