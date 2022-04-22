using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;

public class UtilityTriggerable : BaseTriggerable
{
    public async Task TriggerUtility(Vector3 pos, UtilityAbility ability, GameObject attacker)
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

        // looking for itemUsableObject target
        var itemUsableObject = target.GetComponent<IItemUsable<UtilityAbility>>();
        if (itemUsableObject == null)
            return;

        DisplayBattleLog(target, ability);

        _myStats.SetAttacker(true);

        target.GetComponent<IItemUsable<UtilityAbility>>().UseItem(ability);
    }
}
