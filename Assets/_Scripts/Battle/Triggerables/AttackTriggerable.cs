using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

public class AttackTriggerable : BaseTriggerable
{
    public async Task<GameObject> Attack(Vector3 pos, Ability ability, bool isRetaliation)
    {
        DisplayBattleLog(ability);

        // triggered only once if AOE
        if (!_myStats.IsAttacker)
        {
            await _characterRendererManager.AttackAnimation(ability, pos);

            if (ability.AbilityEffect != null)
            {
                AbilityEffect e = Instantiate(ability.AbilityEffect, pos, Quaternion.identity).GetComponent<AbilityEffect>();
                await e.Play(ability, pos);
            }

            _myStats.UseMana(ability.ManaCost);
        }

        if (!isRetaliation)
            _myStats.SetAttacker(true);

        List<GameObject> targets = await GetTarget(pos, ability);
        if (targets.Count == 0)
            return null;

        foreach (GameObject target in targets)
        {
            if (target == null)
                return null;

            if (target.TryGetComponent(out CharacterStats stats))
            {
                // damage target // TODO: ugh... this -1 is killing me...
                int damage = -1 * ability.CalculateInteractionResult(_myStats, target.GetComponent<CharacterStats>(), isRetaliation);
                bool wasAttackSuccesful = await target.GetComponent<IAttackable<GameObject, Ability>>().TakeDamage(damage, gameObject, ability);
                // 

                Character opponentCharacter = Instantiate(stats.Character); // in case we killed it
            }
            // if it dies when taking damage
            if (target == null)
                return null;

            if (target.TryGetComponent(out ObjectStats objectStats))
                if (ability.Status != null)
                    await objectStats.AddStatus(ability.Status, gameObject);
        }

        if (_myStats.CurrentHealth <= 0) // if we died due to retaliation
            await HighlightManager.Instance.ClearHighlightedTiles();

        // return what you hit - it is only used for position so no worries xDDDDD
        return targets[0];
    }


    async Task<List<GameObject>> GetTarget(Vector3 pos, Ability ability)
    {
        List<GameObject> targets = new();

        // spawn and fire a projectile if the ability has one
        if (ability.Projectile != null)
        {
            GameObject hit = null;
            GameObject projectile = Instantiate(ability.Projectile, transform.position, Quaternion.identity);
            hit = await projectile.GetComponent<Projectile>().Shoot(transform, pos);
            if (hit != null)
            {
                targets.Add(hit);
                return targets;
            }
        }

        // looking for a target
        Collider2D[] cols = Physics2D.OverlapCircleAll(pos, 0.2f);
        foreach (Collider2D c in cols)
            if (c.TryGetComponent(out BaseStats stats))
                targets.Add(c.gameObject);

        return targets;
    }
}
