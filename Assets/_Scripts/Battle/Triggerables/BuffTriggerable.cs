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

        // looking for a target
        Collider2D col = Physics2D.OverlapCircle(pos, 0.2f);
        if (col == null)
            return;
        target = col.gameObject;

        // looking for buffable target
        var buffableObject = target.GetComponent<IBuffable<GameObject, Ability>>();
        if (buffableObject == null)
            return;

        DisplayBattleLog(target, ability);

        _myStats.SetAttacker(true);
        target.GetComponent<IBuffable<GameObject, Ability>>().GetBuffed(attacker, ability);
    }
}
