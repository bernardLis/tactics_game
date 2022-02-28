using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;

public class UtilityTriggerable : BaseTriggerable
{
    public async Task<bool> TriggerUtility(GameObject target, UtilityAbility ability, GameObject attacker)
    {
        if (target == null)
            return false;

        // triggered only once if AOE
        if (!_myStats.IsAttacker)
        {
            // buffing self, should be able to choose what direction to face
            if (target == gameObject && attacker.CompareTag("Player"))
                if (!await PlayerFaceDirSelection()) // allows to break out from selecing face direction
                    return false;

            await _characterRendererManager.SpellcastAnimation();

            _myStats.UseMana(ability.ManaCost);
        }

        // there is item usable, it is being checked in ability;
        target.GetComponent<IItemUsable<UtilityAbility>>().UseItem(ability);

        return true;
    }
}
