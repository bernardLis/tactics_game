using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealExecutor : AbilityExecutor
{
    protected override IEnumerator ExecuteAbilityCoroutine()
    {
        foreach (BattleEntity entity in _entitiesInArea)
            _damageDealt += entity.GetHealed(_selectedAbility);
        CreateBattleLog();

        yield return new WaitForSeconds(3f);
        CancelAbility();
    }

}
