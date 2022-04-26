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
        target = GetTarget(pos);
        if (target == null)
            return;

        DisplayBattleLog(target, ability);

        _myStats.SetAttacker(true);

        target.GetComponent<IItemUsable<UtilityAbility>>().UseItem(ability);
    }

    GameObject GetTarget(Vector3 pos)
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(pos, 0.2f);
        // looking for IItemUsable target
        foreach (Collider2D c in cols)
            if (c.TryGetComponent(out IItemUsable<UtilityAbility> itemUsable))
                return c.gameObject;
        return null;

    }
}
