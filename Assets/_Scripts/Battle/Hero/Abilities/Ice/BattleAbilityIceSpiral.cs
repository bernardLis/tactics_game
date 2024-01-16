using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAbilityIceSpiral : BattleAbility
{
    [SerializeField] GameObject _spiralPrefab;
    List<BattleIceSpiral> _spirals = new();

    public override void Initialize(Ability ability, bool startAbility = true)
    {
        base.Initialize(ability, startAbility);
        transform.localPosition = new Vector3(0f, 0f, 0f); // it is where the effect spawns...
        transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
    }

    protected override IEnumerator ExecuteAbilityCoroutine()
    {
        yield return base.ExecuteAbilityCoroutine();

        BattleIceSpiral spiral = GetInactiveSpiral();
        spiral.gameObject.SetActive(true);
        spiral.Fire(transform.position);
    }

    BattleIceSpiral GetInactiveSpiral()
    {
        foreach (BattleIceSpiral spiral in _spirals)
            if (!spiral.gameObject.activeSelf)
                return spiral;
        return InitializeSpiral();
    }

    BattleIceSpiral InitializeSpiral()
    {
        GameObject instance = Instantiate(_spiralPrefab, Vector3.zero, Quaternion.identity, _battleManager.AbilityHolder);

        BattleIceSpiral spiral = instance.GetComponent<BattleIceSpiral>();
        spiral.Initialize(_ability);
        _spirals.Add(spiral);
        return spiral;
    }
}
