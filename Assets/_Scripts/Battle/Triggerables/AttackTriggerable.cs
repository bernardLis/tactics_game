using UnityEngine;
using System.Threading.Tasks;

public class AttackTriggerable : BaseTriggerable
{
    public async Task Attack(Vector3 pos, Ability ability, bool isRetaliation)
    {
        // triggered only once if AOE
        if (!_myStats.IsAttacker)
        {
            await _characterRendererManager.AttackAnimation();
            _myStats.UseMana(ability.ManaCost);
        }

        GameObject target = await GetTarget(pos, ability);
        if (target == null)
            return;

        DisplayBattleLog(target, ability);

        if (!isRetaliation)
            _myStats.SetAttacker(true);

        // damage target // TODO: ugh... this -1 is killing me...
        int damage = -1 * ability.CalculateInteractionResult(_myStats, target.GetComponent<CharacterStats>());
        bool wasAttackSuccesful = await target.GetComponent<IAttackable<GameObject, Ability>>().TakeDamage(damage, gameObject, ability);

        if (wasAttackSuccesful)
            _myStats.Character.GetExp(target);
    }


    async Task<GameObject> GetTarget(Vector3 pos, Ability ability)
    {
        // spawn and fire a projectile if the ability has one
        if (ability.Projectile != null)
        {
            // spawn projectile in the tile in the direction towards the target
            Vector3 dirToTarget = (pos - transform.position).normalized;
            Vector3 spawLocation = transform.position + dirToTarget;
            GameObject projectile = Instantiate(ability.Projectile, spawLocation, Quaternion.identity);
            Transform hit = await projectile.GetComponent<Projectile>().Shoot(transform, pos);
            if (hit == null)
                return null;
            if (hit.TryGetComponent(out CharacterStats stats))
                return hit.gameObject; // you could have hit someone else, not the one you were aiming at.
            else
                return null;
        }

        // looking for a target
        Collider2D[] cols = Physics2D.OverlapCircleAll(pos, 0.2f);
        // looking for attackable target
        foreach (Collider2D c in cols)
            if (c.TryGetComponent(out IAttackable<GameObject, Ability> attackable))
                return c.gameObject;
        return null;
    }
}
