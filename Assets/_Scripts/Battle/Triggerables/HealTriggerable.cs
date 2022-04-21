using UnityEngine;
using System.Threading.Tasks;

public class HealTriggerable : BaseTriggerable
{
    public async Task Heal(GameObject target, Ability ability, GameObject attacker)
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
    
        int healAmount = ability.CalculateInteractionResult(_myStats, target.GetComponent<CharacterStats>());
        target.GetComponent<IHealable<GameObject, Ability>>().GainHealth(healAmount, gameObject, ability);
    }
}
