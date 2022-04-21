using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;

public class BuffTriggerable : BaseTriggerable
{
    public async Task Buff(GameObject target, Ability ability, GameObject attacker)
    {
        // triggered only once if AOE
        if (!_myStats.IsAttacker)
        {
            await _characterRendererManager.SpellcastAnimation();
            _myStats.UseMana(ability.ManaCost);
        }
        _myStats.SetAttacker(true);

        if (target == null)
            return;

        target.GetComponent<IBuffable<GameObject, Ability>>().GetBuffed(attacker, ability);
    }
}
