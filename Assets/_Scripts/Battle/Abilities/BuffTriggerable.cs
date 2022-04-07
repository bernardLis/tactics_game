using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;

public class BuffTriggerable : BaseTriggerable
{
    public async Task<bool> Buff(GameObject target, Ability ability, GameObject attacker)
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

        target.GetComponent<IBuffable<GameObject, Ability>>().GetBuffed(attacker, ability);

        return true;
    }
}
