using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAbilityWildboltStorm : BattleAbility
{
    [SerializeField] GameObject _effect;

    [SerializeField] GameObject _wildboltPrefab;
    List<BattleWildbolt> _wildboltPool = new();

    public override void Initialize(Ability ability, bool startAbility = true)
    {
        base.Initialize(ability, startAbility);
        transform.localPosition = new Vector3(0, 0.5f, 0f);
    }


    protected override IEnumerator FireAbilityCoroutine()
    {
        yield return base.FireAbilityCoroutine();

        _effect.SetActive(true);
        _effect.transform.parent = transform;
        Vector3 pos = transform.position;
        pos.y = 0.1f;
        _effect.transform.position = pos;
        yield return new WaitForSeconds(0.6f);
        _effect.transform.parent = _battleManager.AbilityHolder;

        int projectileCount = _ability.GetAmount();
        for (int i = 0; i < projectileCount; i++)
        {
            BattleWildbolt projectile = GetInactiveProjectile();
            projectile.transform.position = transform.position;

            projectile.Fire(_ability);
            // yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(2f);

        _effect.SetActive(false);
    }

    BattleWildbolt InitializeProjectile()
    {
        GameObject instance = Instantiate(_wildboltPrefab, Vector3.zero, Quaternion.identity, _battleManager.AbilityHolder);
        instance.SetActive(true);

        BattleWildbolt projectile = instance.GetComponent<BattleWildbolt>();
        projectile.Initialize(0);
        _wildboltPool.Add(projectile);
        return projectile;
    }

    BattleWildbolt GetInactiveProjectile()
    {
        foreach (BattleWildbolt p in _wildboltPool)
            if (!p.gameObject.activeSelf)
                return p;
        return InitializeProjectile();
    }
}
