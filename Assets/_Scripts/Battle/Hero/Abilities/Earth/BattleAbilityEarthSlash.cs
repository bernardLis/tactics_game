using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using PlasticPipe.PlasticProtocol.Messages;

public class BattleAbilityEarthSlash : BattleAbility
{
    [SerializeField] GameObject _slashPrefab;
    List<BattleEarthSlash> _slashPool = new();

    public override void Initialize(Ability ability, bool startAbility = true)
    {
        base.Initialize(ability, startAbility);
        transform.localPosition = new Vector3(0f, 0f, 1f); // it is where the effect spawns...
    }

    protected override IEnumerator FireAbilityCoroutine()
    {
        yield return base.FireAbilityCoroutine();
        for (int i = 0; i < _ability.GetAmount(); i++)
        {
            BattleEarthSlash s = GetInactiveSlash();
            s.Fire(GetSlashPosition(i), GetSlashRotation(i));
        }
    }

    BattleEarthSlash InitializeSlash()
    {
        GameObject instance = Instantiate(_slashPrefab, Vector3.zero,
                                Quaternion.identity, _battleManager.AbilityHolder);

        BattleEarthSlash slash = instance.GetComponent<BattleEarthSlash>();
        slash.Initialize(_ability);
        _slashPool.Add(slash);
        return slash;
    }

    BattleEarthSlash GetInactiveSlash()
    {
        foreach (BattleEarthSlash p in _slashPool)
            if (!p.gameObject.activeSelf)
                return p;
        return InitializeSlash();
    }

    Vector3 GetSlashPosition(int i)
    {
        if (i == 0)
            return Vector3.up * 0.5f + Vector3.forward;
        if (i == 1)
            return Vector3.up * 0.5f + Vector3.forward * -1;

        return transform.position;
    }

    Quaternion GetSlashRotation(int i)
    {
        if (i == 1)
            return Quaternion.Euler(0f, 180f, 0f);
        return Quaternion.identity;
    }

}
