using UnityEngine;
using System.Threading.Tasks;

public class AttackTriggerable : BaseTriggerable
{
    public async Task<bool> Attack(GameObject target, Ability ability, bool isRetaliation)
    {
        if (target == null)
            return false;

        // triggered only once if AOE
        if (!_myStats.IsAttacker)
        {
            await _characterRendererManager.AttackAnimation();

            // spawn and fire a projectile if the ability has one
            if (ability.Projectile != null)
            {
                GameObject projectile = Instantiate(ability.Projectile, transform.position, Quaternion.identity);
                projectile.GetComponent<IShootable<Transform>>().Shoot(target.transform);

                // TODO: There is a better way to wait for shoot to hit the target;
                await Task.Delay(300);
            }

            _myStats.UseMana(ability.ManaCost);
        }

        if (!isRetaliation)
            _myStats.SetAttacker(true);

        // damage target
        int damage = ability.BasePower + _myStats.Strength.GetValue();
        await target.GetComponent<IAttackable<GameObject, Ability>>().TakeDamage(damage, gameObject, ability);

        return true;
    }
}
