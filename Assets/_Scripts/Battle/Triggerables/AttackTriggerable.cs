using UnityEngine;
using System.Threading.Tasks;

public class AttackTriggerable : BaseTriggerable
{
    public async Task<GameObject> Attack(Vector3 pos, Ability ability, bool isRetaliation)
    {
        // triggered only once if AOE
        if (!_myStats.IsAttacker)
        {
            // TODO: may be problematic, play spellcast animation on spell abilities
            if (ability.MultiplerStat == StatType.Intelligence || ability.SpellcastAnimation)
                await _characterRendererManager.SpellcastAnimation();
            else
                await _characterRendererManager.AttackAnimation();

            if (ability.AbilityEffect != null)
            {
                Effect e = Instantiate(ability.AbilityEffect, pos, Quaternion.identity).GetComponent<Effect>();
                await e.Play(ability, pos);
                //e.DestroySelf();
            }

            _myStats.UseMana(ability.ManaCost);
        }

        if (!isRetaliation)
            _myStats.SetAttacker(true);

        GameObject target = await GetTarget(pos, ability);
        if (target == null)
            return null;

        if (target.TryGetComponent(out CharacterStats stats))
        {
            DisplayBattleLog(target, ability);

            // damage target // TODO: ugh... this -1 is killing me...
            int damage = -1 * ability.CalculateInteractionResult(_myStats, target.GetComponent<CharacterStats>());
            bool wasAttackSuccesful = await target.GetComponent<IAttackable<GameObject, Ability>>().TakeDamage(damage, gameObject, ability);

            if (wasAttackSuccesful)
                _myStats.Character.GetExp(target);
        }

        if (target.TryGetComponent(out ObjectStats objectStats))
        {
            if (ability.Status != null)
                objectStats.AddStatus(ability.Status, gameObject);
        }

        // return what you hit
        return target;
    }


    async Task<GameObject> GetTarget(Vector3 pos, Ability ability)
    {
        // spawn and fire a projectile if the ability has one
        if (ability.Projectile != null)
        {
            GameObject projectile = Instantiate(ability.Projectile, transform.position, Quaternion.identity);
            return await projectile.GetComponent<Projectile>().Shoot(transform, pos);
        }

        // looking for a target
        Collider2D[] cols = Physics2D.OverlapCircleAll(pos, 0.2f);
        // looking for attackable target
        foreach (Collider2D c in cols)
            if (c.TryGetComponent(out BaseStats stats))
                return c.gameObject;
        return null;
    }
}
