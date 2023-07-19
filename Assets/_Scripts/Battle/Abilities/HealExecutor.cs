using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealExecutor : AbilityExecutor
{

    public override void ExecuteAbility(Ability ability)
    {
        base.ExecuteAbility(ability);

        _effectInstance.transform.localScale = Vector3.one * _selectedAbility.GetScale();

        foreach (ParticleSystem ps in _effectInstance.GetComponentsInChildren<ParticleSystem>())
        {
            var burst = ps.emission.GetBurst(0);
            burst.count = ability.Level * 2;
            ps.emission.SetBurst(0, burst);
        }
    }

    protected override IEnumerator ExecuteAbilityCoroutine()
    {
        foreach (BattleEntity entity in _entitiesInArea)
            _damageDealt += entity.GetHealed(_selectedAbility);

        yield return new WaitForSeconds(3f);
        CancelAbility();
    }

}
