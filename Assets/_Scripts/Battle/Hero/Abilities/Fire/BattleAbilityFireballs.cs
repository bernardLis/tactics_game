using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
public class BattleAbilityFireballs : BattleAbility
{
    [SerializeField] GameObject _fireballPrefab;
    List<BattleProjectile> _fireballPool = new();

    public override void Initialize(Ability ability, bool startAbility)
    {
        base.Initialize(ability, startAbility);
        transform.localPosition = new Vector3(0f, 0.5f, 0f);
    }

    BattleProjectile InitializeFireball()
    {
        GameObject instance = Instantiate(_fireballPrefab, Vector3.zero, Quaternion.identity,
                                _battleManager.AbilityHolder);
        instance.SetActive(true);

        BattleProjectile projectile = instance.GetComponent<BattleProjectile>();
        projectile.Initialize(0);
        _fireballPool.Add(projectile);
        return projectile;
    }

    protected override IEnumerator ExecuteAbilityCoroutine()
    {
        yield return base.ExecuteAbilityCoroutine();
        Vector3 rand = Random.insideUnitCircle;
        Vector3 dir = new Vector3(rand.x, 0, rand.y);
        Vector3 projectileVariance = new Vector3(0, 0, 0.07f);
        for (int i = 0; i < _ability.GetAmount(); i++)
        {
            BattleProjectile projectile = GetInactiveFireball();
            projectile.transform.localScale = Vector3.one * _ability.GetScale();
            projectile.transform.position = transform.position;
            projectile.Shoot(_ability, dir + projectileVariance * i);
        }
    }

    BattleProjectile GetInactiveFireball()
    {
        foreach (BattleProjectile ball in _fireballPool)
            if (!ball.gameObject.activeSelf)
                return ball;
        return InitializeFireball();
    }
}
