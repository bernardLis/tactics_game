using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using PlasticPipe.PlasticProtocol.Messages;

public class BattleAbilityEarthSlash : BattleAbility
{
    [SerializeField] GameObject _slashPrefab;
    List<BattleEarthSlash> _slashes = new();

    public override void Initialize(Ability ability, bool startAbility = true)
    {
        base.Initialize(ability, startAbility);
        transform.localPosition = new Vector3(0f, 0f, 1f); // it is where the effect spawns...
        transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

        ability.OnLevelUp += OnAbilityLevelUp;
        for (int i = 0; i < _ability.GetAmount(); i++) // HERE: amount of ability
            InitializeSlash();
    }

    void OnAbilityLevelUp()
    {
        int missingSlashes = _ability.GetAmount() - _slashes.Count;
        for (int i = 0; i < missingSlashes; i++)
            InitializeSlash();
    }

    void InitializeSlash()
    {
        GameObject instance = Instantiate(_slashPrefab, transform);
        instance.transform.localPosition = GetSlashPosition();
        instance.transform.localRotation = GetSlashRotation();
        instance.SetActive(true);

        BattleEarthSlash battleStoneSlash = instance.GetComponent<BattleEarthSlash>();
        battleStoneSlash.Initialize(_ability, this);

        _slashes.Add(battleStoneSlash);
    }

    Vector3 GetSlashPosition()
    {
        if (_slashes.Count == 0)
            return Vector3.up * 0.5f + Vector3.forward;
        if (_slashes.Count == 1)
            return Vector3.up * 0.5f + Vector3.forward * -1;

        return transform.position;
    }

    Quaternion GetSlashRotation()
    {
        if (_slashes.Count == 1)
        {
            return Quaternion.Euler(0f, 180f, 0f);
        }
        return Quaternion.identity;
    }

    protected override IEnumerator FireAbilityCoroutine()
    {
        yield return base.FireAbilityCoroutine();
        foreach (BattleEarthSlash slash in _slashes)
            slash.Fire();
    }
}
