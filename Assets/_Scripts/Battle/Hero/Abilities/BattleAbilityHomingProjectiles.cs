using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAbilityHomingProjectiles : BattleAbility
{
    [SerializeField] GameObject _projectilePrefab;

    public override void Initialize(Ability ability)
    {
        base.Initialize(ability);
        transform.localPosition = new Vector3(0, 1.6f, 0f);
    }

    protected override IEnumerator FireAbilityCoroutine()
    {
        yield return base.FireAbilityCoroutine();

        // fire projectile at enemy team fire them imperfectly - so they are not underneath each other
        int projectileCount = Mathf.RoundToInt(_ability.GetScale());
        for (int i = 0; i < projectileCount; i++)
        {

            Quaternion q = Quaternion.Euler(Random.Range(180f, 360f),
                                            Random.Range(0f, 360f),
                                            Random.Range(0f, 360f));

            GameObject projectileInstance = Instantiate(_projectilePrefab,
                                        transform.position, q);
            projectileInstance.transform.SetParent(transform);

            BattleProjectileHoming projectile = projectileInstance.GetComponent<BattleProjectileHoming>();
            projectile.Initialize(0); // hardcoded for now
            projectile.StartHoming(_ability);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
