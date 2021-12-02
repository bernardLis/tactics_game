using UnityEngine;
using System.Threading.Tasks;

public class AttackTriggerable : MonoBehaviour
{
    public Transform projectileSpawnPoint; // TODO: is that ok way to handle this?

    CharacterStats myStats;
    CharacterRendererManager characterRendererManager;

    bool hasPlayedAnimation;

    void Awake()
    {
        myStats = GetComponent<CharacterStats>();
        characterRendererManager = GetComponentInChildren<CharacterRendererManager>();
    }

    public async Task<bool> Attack(GameObject _target, int _value, int _manaCost, GameObject _projectile)
    {
        // TODO: !hasPlayedAnimation kinda sucks
        if (!hasPlayedAnimation)
        {
            // play animation
            Vector2 dir = _target.transform.position - transform.position;
            await characterRendererManager.AttackAnimation(dir);

            // spawn and fire a projectile if the ability has one
            if (_projectile != null)
            {
                GameObject projectile = Instantiate(_projectile, transform.position, Quaternion.identity);
                projectile.GetComponent<IShootable<Transform>>().Shoot(_target.transform);

                // TODO: There is a better way to wait for shoot to hit the target;
                await Task.Delay(300);
            }

            // reseted by char selection - this makes sure you play only one animation per attack - useful for aoe attacks
            if (myStats.isAttacker) // to not set this when you retaliate
                hasPlayedAnimation = true;
        }

        // damage target
        int damage = _value + myStats.strength.GetValue();

        myStats.SetAttacker(true);
        myStats.UseMana(_manaCost);

        await _target.GetComponent<IAttackable<GameObject>>().TakeDamage(damage, gameObject);

        return true;
    }

    public void SetHasPlayedAnimation(bool _has) { hasPlayedAnimation = _has; }
}
