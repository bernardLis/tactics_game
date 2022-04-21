using UnityEngine;
using System.Threading.Tasks;

public class AttackTriggerable : BaseTriggerable
{
    public async Task Attack(GameObject target, Ability ability, bool isRetaliation)
    {
        // triggered only once if AOE
        if (!_myStats.IsAttacker)
        {
            await _characterRendererManager.AttackAnimation();
            _myStats.UseMana(ability.ManaCost);

            // spawn and fire a projectile if the ability has one
            if (ability.Projectile != null)
            {
                GameObject projectile = Instantiate(ability.Projectile, transform.position, Quaternion.identity);
                Transform hit = await projectile.GetComponent<Projectile>().Shoot(transform, target.transform);
                if (!hit.TryGetComponent(out CharacterStats stats))
                    return;

                // you could have hit someone else, not the one you were aiming at.
                target = hit.gameObject;
            }
        }

        if (!isRetaliation)
            _myStats.SetAttacker(true);

        // trigger ability even if there is no target (play attack animation & use mana even if there is no target)
        if (target == null)
            return;

        // damage target // TODO: ugh... this -1 is killing me...
        int damage = -1 * ability.CalculateInteractionResult(_myStats, target.GetComponent<CharacterStats>());
        await target.GetComponent<IAttackable<GameObject, Ability>>().TakeDamage(damage, gameObject, ability);
    }
}
