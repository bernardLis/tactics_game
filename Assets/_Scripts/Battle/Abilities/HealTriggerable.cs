using UnityEngine;
using System.Threading.Tasks;

public class HealTriggerable : BaseTriggerable
{
    // returns true if successfully healed
    public async Task<bool> Heal(GameObject target, Ability ability, GameObject attacker)
    {
        // triggered only once if AOE
        if (!_myStats.IsAttacker)
        {
            // healing self, should be able to choose what direction to face
            if (target == gameObject && attacker.CompareTag("Player"))
                if (!await PlayerFaceDirSelection()) // allows to break out from selecing face direction
                    return false;

            // animation
            await _characterRendererManager.SpellcastAnimation();

            _myStats.UseMana(ability.ManaCost);
        }

        // data
        int healAmount = ability.CalculateInteractionResult(_myStats, target.GetComponent<CharacterStats>());
        target.GetComponent<IHealable<GameObject, Ability>>().GainHealth(healAmount, gameObject, ability);

        _myStats.SetAttacker(true);

        return true;
    }
}
