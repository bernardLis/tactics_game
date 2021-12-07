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

    public async Task<bool> Attack(GameObject _target, int _value, int _manaCost, GameObject _projectile, StatModifier _modifier,
                                   bool _isRetaliation)
    {
        if (_target == null)
            return false;

        // triggered only once if AOE
        if (!myStats.isAttacker)
        {
            await characterRendererManager.AttackAnimation();

            // spawn and fire a projectile if the ability has one
            if (_projectile != null)
            {
                GameObject projectile = Instantiate(_projectile, transform.position, Quaternion.identity);
                projectile.GetComponent<IShootable<Transform>>().Shoot(_target.transform);

                // TODO: There is a better way to wait for shoot to hit the target;
                await Task.Delay(300);
            }

            myStats.UseMana(_manaCost);
        }

        if (!_isRetaliation)
            myStats.SetAttacker(true);

        // damage target
        int damage = _value + myStats.strength.GetValue();
        await _target.GetComponent<IAttackable<GameObject>>().TakeDamage(damage, gameObject);

        // adding stat modifiers
        List<Stat> stats = _target.GetComponent<CharacterStats>().stats;
        foreach (Stat s in stats)
            if (s.type == _modifier.statType)
                s.AddModifier(Instantiate(_modifier));

        return true;
    }
}
