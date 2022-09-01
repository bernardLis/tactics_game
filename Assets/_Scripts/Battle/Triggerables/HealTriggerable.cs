using UnityEngine;
using System.Threading.Tasks;

public class HealTriggerable : BaseTriggerable
{
    public async Task Heal(Vector3 pos, Ability ability, GameObject attacker)
    {
        DisplayBattleLog(ability);

        GameObject target;
        // triggered only once if AOE
        if (!_myStats.IsAttacker)
        {
            await _characterRendererManager.SpellcastAnimation();
            _myStats.UseMana(ability.ManaCost);
        }
        _myStats.SetAttacker(true);

        // looking for a target
        target = GetTarget(pos);
        if (target == null)
            return;

        int healAmount = ability.CalculateInteractionResult(_myStats, target.GetComponent<CharacterStats>());
        target.GetComponent<IHealable<GameObject, Ability>>().GainHealth(healAmount, gameObject, ability);
    }

    GameObject GetTarget(Vector3 pos)
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(pos, 0.2f);
        // looking for healable target
        foreach (Collider2D c in cols)
            if (c.TryGetComponent(out IHealable<GameObject, Ability> healable))
                return c.gameObject;
        return null;

    }

}
