using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAbilityIceSpiral : BattleAbility
{
    public override void Initialize(Ability ability, bool startAbility = true)
    {
        base.Initialize(ability, startAbility);
        transform.localPosition = Vector3.zero;
    }

    protected override IEnumerator ExecuteAbilityCoroutine()
    {
        yield return base.ExecuteAbilityCoroutine();

        BattleIceSpiral spiral = GetInactiveAbilityObject() as BattleIceSpiral;
        spiral.Execute(transform.position, Quaternion.identity);
    }
}
