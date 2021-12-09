using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;

public class AttackTriggerable : MonoBehaviour
{
    public Transform projectileSpawnPoint; // TODO: is that ok way to handle this?

    CharacterStats myStats;
    CharacterRendererManager characterRendererManager;

    void Awake()
    {
        myStats = GetComponent<CharacterStats>();
        characterRendererManager = GetComponentInChildren<CharacterRendererManager>();
    }

    public async Task<bool> Attack(GameObject _target, Ability _ability, bool _isRetaliation)// int _value, int _manaCost, GameObject _projectile, StatModifier _modifier,
                                   //Status _status)
    {
        if (_target == null)
            return false;

        // triggered only once if AOE
        if (!myStats.isAttacker)
        {
            await characterRendererManager.AttackAnimation();

            // spawn and fire a projectile if the ability has one
            if (_ability.aProjectile != null)
            {
                GameObject projectile = Instantiate(_ability.aProjectile, transform.position, Quaternion.identity);
                projectile.GetComponent<IShootable<Transform>>().Shoot(_target.transform);

                // TODO: There is a better way to wait for shoot to hit the target;
                await Task.Delay(300);
            }

            myStats.UseMana(_ability.manaCost);
        }

        if (!_isRetaliation)
            myStats.SetAttacker(true);

        // damage target
        int damage = _ability.value + myStats.strength.GetValue();
        await _target.GetComponent<IAttackable<GameObject, Ability>>().TakeDamage(damage, gameObject, _ability);

        return true;
    }
}
