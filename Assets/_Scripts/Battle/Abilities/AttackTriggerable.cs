using UnityEngine;
using System.Threading.Tasks;

public class AttackTriggerable : MonoBehaviour
{
    public Transform ProjectileSpawnPoint; // TODO: is that ok way to handle this?

    CharacterStats _characterStats;
    CharacterRendererManager _characterRendererManager;

    void Awake()
    {
        _characterStats = GetComponent<CharacterStats>();
        _characterRendererManager = GetComponentInChildren<CharacterRendererManager>();
    }

    public async Task<bool> Attack(GameObject target, Ability ability, bool isRetaliation)
    {
        if (target == null)
            return false;

        // triggered only once if AOE
        if (!_characterStats.isAttacker)
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

            _characterStats.UseMana(ability.ManaCost);
        }

        if (!isRetaliation)
            _characterStats.SetAttacker(true);

        // damage target
        int damage = ability.BasePower + _characterStats.strength.GetValue();
        await target.GetComponent<IAttackable<GameObject, Ability>>().TakeDamage(damage, gameObject, ability);

        return true;
    }
}
