using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using PlasticPipe.PlasticProtocol.Messages;

public class BattleAbilityEarthSpike : BattleAbility
{
    [SerializeField] GameObject _spikePrefab;
    List<BattleEarthSpike> _spikes = new();

    public override void Initialize(Ability ability, bool startAbility = true)
    {
        base.Initialize(ability, startAbility);
        transform.localPosition = new Vector3(0f, 0f, 4f); // it is where the effect spawns...
        transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

        InitializeSpike();
    }

    BattleEarthSpike InitializeSpike()
    {
        GameObject instance = Instantiate(_spikePrefab, Vector3.zero, Quaternion.identity, BattleManager.Instance.EntityHolder);
        instance.SetActive(true);

        BattleEarthSpike battleEarthSpike = instance.GetComponent<BattleEarthSpike>();
        battleEarthSpike.Initialize(_ability, this);
        _spikes.Add(battleEarthSpike);
        return battleEarthSpike;
    }

    protected override IEnumerator FireAbilityCoroutine()
    {
        yield return base.FireAbilityCoroutine();
        foreach (BattleEarthSpike spike in _spikes)
        {
            if (!spike.IsActive)
            {
                spike.Fire(transform.position, transform.rotation.eulerAngles);
                yield break;
            }
        }
        BattleEarthSpike battleEarthSpike = InitializeSpike();
        battleEarthSpike.Fire(transform.position, transform.rotation.eulerAngles);

    }
}
