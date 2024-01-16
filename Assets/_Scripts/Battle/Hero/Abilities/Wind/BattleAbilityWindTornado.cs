using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleAbilityWindTornado : BattleAbility
{
    [SerializeField] GameObject _tornadoPrefab;
    List<BattleWindTornado> _tornadoPool = new();

    public override void Initialize(Ability ability, bool startAbility = true)
    {
        base.Initialize(ability, startAbility);
        transform.localPosition = new Vector3(0.5f, 1f, 0.5f);
    }

    protected override IEnumerator ExecuteAbilityCoroutine()
    {
        yield return base.ExecuteAbilityCoroutine();

        for (int i = 0; i < _ability.GetAmount(); i++)
        {
            Vector3 pos = new(transform.position.x, 0, transform.position.z);
            Quaternion q = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

            BattleWindTornado tornado = GetInactiveTornado();
            tornado.Fire(pos, q);
            yield return new WaitForSeconds(0.2f);
        }
    }

    BattleWindTornado InitializeTornado()
    {
        GameObject instance = Instantiate(_tornadoPrefab, Vector3.zero, Quaternion.identity, _battleManager.AbilityHolder);
        BattleWindTornado tornado = instance.GetComponent<BattleWindTornado>();
        tornado.Initialize(_ability);
        _tornadoPool.Add(tornado);
        return tornado;
    }

    BattleWindTornado GetInactiveTornado()
    {
        foreach (BattleWindTornado p in _tornadoPool)
            if (!p.gameObject.activeSelf)
                return p;
        return InitializeTornado();
    }
}
