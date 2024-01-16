using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAbilityWaterTornado : BattleAbility
{
    [SerializeField] GameObject _tornadoPrefab;
    List<BattleWaterTornado> _tornadoPool = new();

    public override void Initialize(Ability ability, bool startAbility = true)
    {
        base.Initialize(ability, startAbility);
        transform.localPosition = new Vector3(0.5f, 1f, 0f);
    }

    protected override IEnumerator ExecuteAbilityCoroutine()
    {
        yield return base.ExecuteAbilityCoroutine();

        for (int i = 0; i < _ability.GetAmount(); i++)
        {
            Vector3 pos = _battleAreaManager.GetRandomPositionWithinRangeOnActiveTile(transform.position,
                            Random.Range(7, 14));
            BattleWaterTornado tornado = GetInactiveTornado();
            tornado.Fire(pos);
            yield return new WaitForSeconds(0.1f);
        }
    }

    BattleWaterTornado InitializeTornado()
    {
        GameObject instance = Instantiate(_tornadoPrefab, Vector3.zero, Quaternion.identity, _battleManager.AbilityHolder);
        BattleWaterTornado tornado = instance.GetComponent<BattleWaterTornado>();
        tornado.Initialize(_ability);
        _tornadoPool.Add(tornado);
        return tornado;
    }

    BattleWaterTornado GetInactiveTornado()
    {
        foreach (BattleWaterTornado p in _tornadoPool)
            if (!p.gameObject.activeSelf)
                return p;
        return InitializeTornado();
    }
}
