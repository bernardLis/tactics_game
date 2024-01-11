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
        GameObject instance = Instantiate(_fireballPrefab, Vector3.zero, Quaternion.identity, BattleManager.Instance.EntityHolder);
        instance.SetActive(true);

        BattleProjectile projectile = instance.GetComponent<BattleProjectile>();
        projectile.Initialize(0);
        _fireballPool.Add(projectile);
        return projectile;
    }

    protected override IEnumerator FireAbilityCoroutine()
    {
        yield return base.FireAbilityCoroutine();

        Vector3 dir = transform.position + Random.insideUnitSphere;
        dir.y = 0;
        // Vector3 r = new(0, Random.Range(0f, 360f), 0);
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
