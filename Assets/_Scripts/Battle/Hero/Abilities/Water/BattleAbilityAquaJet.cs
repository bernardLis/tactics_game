using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAbilityAquaJet : BattleAbility
{
    [SerializeField] GameObject _projectilePrefab;
    List<BattleProjectileHoming> _projectilePool = new();

    public override void Initialize(Ability ability, bool startAbility = true)
    {
        base.Initialize(ability, startAbility);
        transform.localPosition = new Vector3(0.5f, 1f, 0f);
    }

    protected override IEnumerator ExecuteAbilityCoroutine()
    {
        yield return base.ExecuteAbilityCoroutine();

        int projectileCount = _ability.GetAmount();
        for (int i = 0; i < projectileCount; i++)
        {

            Quaternion q = Quaternion.Euler(Random.Range(180f, 360f),
                                            Random.Range(0f, 360f),
                                            Random.Range(0f, 360f));

            BattleProjectileHoming projectile = GetInactiveProjectile();
            projectile.transform.position = transform.position;
            projectile.transform.rotation = q;

            projectile.StartHoming(_ability);
            yield return new WaitForSeconds(0.1f);
        }
    }

    BattleProjectileHoming InitializeProjectile()
    {
        GameObject instance = Instantiate(_projectilePrefab, Vector3.zero, Quaternion.identity, _battleManager.AbilityHolder);
        instance.SetActive(true);

        BattleProjectileHoming projectile = instance.GetComponent<BattleProjectileHoming>();
        projectile.Initialize(0);
        _projectilePool.Add(projectile);
        return projectile;
    }

    BattleProjectileHoming GetInactiveProjectile()
    {
        foreach (BattleProjectileHoming p in _projectilePool)
            if (!p.gameObject.activeSelf)
                return p;
        return InitializeProjectile();
    }


}
