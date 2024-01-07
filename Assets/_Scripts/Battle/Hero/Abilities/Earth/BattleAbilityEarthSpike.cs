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
        GameObject instance = Instantiate(_spikePrefab, Vector3.zero, Quaternion.identity);
        instance.SetActive(true);

        BattleEarthSpike battleEarthSpike = instance.GetComponent<BattleEarthSpike>();
        battleEarthSpike.Initialize(_ability, this);
        _spikes.Add(battleEarthSpike);
        return battleEarthSpike;
    }

    protected override IEnumerator FireAbilityCoroutine()
    {
        Debug.Log($"fire ability coroutine {_spikes.Count}");
        yield return base.FireAbilityCoroutine();
        foreach (BattleEarthSpike spike in _spikes)
        {
            Debug.Log($"{spike.name} isActive: {spike.IsActive}");
            if (!spike.IsActive)
            {

                Debug.Log($"found inactive");
                spike.Fire(transform.position, transform.rotation.eulerAngles);
                yield break;
            }
        }
        BattleEarthSpike battleEarthSpike = InitializeSpike();
        battleEarthSpike.Fire(transform.position, transform.rotation.eulerAngles);

    }
}
