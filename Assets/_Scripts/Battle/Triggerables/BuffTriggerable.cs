using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;

public class BuffTriggerable : BaseTriggerable
{
    public async Task Buff(Vector3 pos, Ability ability, GameObject attacker)
    {
        GameObject target;
        // triggered only once if AOE
        if (!_myStats.IsAttacker)
        {
            await _characterRendererManager.SpellcastAnimation();
            _myStats.UseMana(ability.ManaCost);
        }
        target = GetTarget(pos);
        if (target == null)
            return;

        DisplayBattleLog(target, ability);

        _myStats.SetAttacker(true);
        target.GetComponent<IBuffable<GameObject, Ability>>().GetBuffed(attacker, ability);
    }

    GameObject GetTarget(Vector3 pos)
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(pos, 0.2f);
        // looking for buffable target
        foreach (Collider2D c in cols)
            if (c.TryGetComponent(out IBuffable<GameObject, Ability> buffable))
                return c.gameObject;
        return null;
    }
}
