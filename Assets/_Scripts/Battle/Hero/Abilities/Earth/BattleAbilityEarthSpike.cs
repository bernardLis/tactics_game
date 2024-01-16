using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleAbilityEarthSpike : BattleAbility
{
    [SerializeField] GameObject _spikePrefab;
    List<BattleEarthSpike> _spikePool = new();

    public override void Initialize(Ability ability, bool startAbility = true)
    {
        base.Initialize(ability, startAbility);
        transform.localPosition = new Vector3(0f, 0f, 4f); // it is where the effect spawns...
        transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }

    protected override IEnumerator ExecuteAbilityCoroutine()
    {
        yield return base.ExecuteAbilityCoroutine();
        BattleEarthSpike spike = GetInactiveSpike();
        spike.Fire(transform.position, transform.rotation.eulerAngles);
    }

    BattleEarthSpike InitializeSpike()
    {
        GameObject instance = Instantiate(_spikePrefab, Vector3.zero,
                                Quaternion.identity, _battleManager.AbilityHolder);

        BattleEarthSpike spike = instance.GetComponent<BattleEarthSpike>();
        spike.Initialize(_ability);
        _spikePool.Add(spike);
        return spike;
    }

    BattleEarthSpike GetInactiveSpike()
    {
        foreach (BattleEarthSpike p in _spikePool)
            if (!p.gameObject.activeSelf)
                return p;
        return InitializeSpike();
    }
}
