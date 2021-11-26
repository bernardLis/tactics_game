using UnityEngine;
using System.Threading.Tasks;

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

    public async Task<bool> Attack(GameObject target, int value, int manaCost, GameObject _projectile)
    {
        // play animation
        Vector2 dir = target.transform.position - transform.position;
        await characterRendererManager.AttackAnimation(dir);

        // spawn and fire a projectile if the ability has one
        if(_projectile != null)
        {
            GameObject projectile = Instantiate(_projectile, transform.position, Quaternion.identity);
            projectile.GetComponent<IShootable<Transform>>().Shoot(target.transform);

            // TODO: There is a better way to wait for shoot to hit the target;
            await Task.Delay(300);
        }

        // damage target
        int damage = value + myStats.strength.GetValue();

        myStats.SetAttacker(true);
        myStats.UseMana(manaCost);

        await target.GetComponent<IAttackable<GameObject>>().TakeDamage(damage, gameObject);

        return true;
    }
}
