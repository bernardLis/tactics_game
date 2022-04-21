using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;

public class UtilityTriggerable : BaseTriggerable
{
    public async Task TriggerUtility(GameObject target, UtilityAbility ability, GameObject attacker)
    {
        // triggered only once if AOE
        if (!_myStats.IsAttacker)
        {
            await _characterRendererManager.SpellcastAnimation();
            _myStats.UseMana(ability.ManaCost);
        }
        
        if (target == null)
            return;

        // there is item usable, it is being checked in ability;
        target.GetComponent<IItemUsable<UtilityAbility>>().UseItem(ability);
    }
}
