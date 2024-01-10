using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
public class BattleAbilityMeteors : BattleAbility
{

    [SerializeField] GameObject _meteorPrefab;
    List<BattleMeteors> _meteors = new();

    public override void Initialize(Ability ability, bool startAbility)
    {
        base.Initialize(ability, startAbility);
        transform.localPosition = new Vector3(-0.5f, 1f, 0f);
    }

    BattleMeteors InitializeMeteor()
    {
        GameObject instance = Instantiate(_meteorPrefab, Vector3.zero, Quaternion.identity, BattleManager.Instance.EntityHolder);
        instance.SetActive(true);

        BattleMeteors meteors = instance.GetComponent<BattleMeteors>();
        meteors.Initialize(_ability);
        _meteors.Add(meteors);
        return meteors;
    }

    protected override IEnumerator FireAbilityCoroutine()
    {
        yield return base.FireAbilityCoroutine();
        for (int i = 0; i < _ability.GetAmount(); i++)
        {
            // random position within circle radius
            Vector3 pos = _battleAreaManager.GetRandomPositionWithinRangeOnActiveTile(transform.position, Random.Range(10, 20));
            BattleMeteors meteor = GetInactiveMeteor();
            meteor.Fire(pos);
        }
    }

    BattleMeteors GetInactiveMeteor()
    {
        foreach (BattleMeteors meteors in _meteors)
            if (!meteors.IsActive)
                return meteors;
        return InitializeMeteor();
    }
}
