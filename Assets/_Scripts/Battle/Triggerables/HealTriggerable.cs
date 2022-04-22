using UnityEngine;
using System.Threading.Tasks;

public class HealTriggerable : BaseTriggerable
{
    public async Task Heal(Vector3 pos, Ability ability, GameObject attacker)
    {
        GameObject target;
        // triggered only once if AOE
        if (!_myStats.IsAttacker)
        {
            await _characterRendererManager.SpellcastAnimation();
            _myStats.UseMana(ability.ManaCost);
        }
        // looking for a target
        Collider2D col = Physics2D.OverlapCircle(pos, 0.2f);
        if (col == null)
            return;
        target = col.gameObject;

        // looking for healable target
        var healableObject = target.GetComponent<IHealable<GameObject, Ability>>();
        if (healableObject == null)
            return;

        DisplayBattleLog(target, ability);

        _myStats.SetAttacker(true);
        int healAmount = ability.CalculateInteractionResult(_myStats, target.GetComponent<CharacterStats>());
        target.GetComponent<IHealable<GameObject, Ability>>().GainHealth(healAmount, gameObject, ability);
    }
}
