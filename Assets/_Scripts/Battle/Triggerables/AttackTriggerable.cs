using UnityEngine;
using System.Threading.Tasks;

public class AttackTriggerable : BaseTriggerable
{
    public async Task Attack(Vector3 pos, Ability ability, bool isRetaliation)
    {
        GameObject target;
        // triggered only once if AOE
        if (!_myStats.IsAttacker)
        {
            await _characterRendererManager.AttackAnimation();
            _myStats.UseMana(ability.ManaCost);

            // spawn and fire a projectile if the ability has one
            if (ability.Projectile != null)
            {
                GameObject projectile = Instantiate(ability.Projectile, transform.position, Quaternion.identity);
                Transform hit = await projectile.GetComponent<Projectile>().Shoot(transform, pos);
                if (hit == null)
                    return;
                if (!hit.TryGetComponent(out CharacterStats stats))
                    return;

                // you could have hit someone else, not the one you were aiming at.
                target = hit.gameObject;
            }
        }
        // looking for a target
        Collider2D col = Physics2D.OverlapCircle(pos, 0.2f);
        if (col == null)
            return;
        target = col.gameObject;

        // looking for attackable target
        var attackableObject = target.GetComponent<IAttackable<GameObject, Ability>>();
        if (attackableObject == null)
            return;

        DisplayBattleLog(target, ability);

        if (!isRetaliation)
            _myStats.SetAttacker(true);

        // damage target // TODO: ugh... this -1 is killing me...
        int damage = -1 * ability.CalculateInteractionResult(_myStats, target.GetComponent<CharacterStats>());
        await target.GetComponent<IAttackable<GameObject, Ability>>().TakeDamage(damage, gameObject, ability);
    }
}
