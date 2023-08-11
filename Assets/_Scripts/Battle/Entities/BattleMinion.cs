using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMinion : BattleEntity
{
    BattleSpire _spire;
    public Minion Minion { get; private set; }

    public override void InitializeEntity(Entity entity)
    {
        base.InitializeEntity(entity);
        Minion = (Minion)entity;
    }

    public override void InitializeBattle(int team, ref List<BattleEntity> opponents)
    {
        base.InitializeBattle(team, ref opponents);

        _spire = BattleSpire.Instance;

        StartRunEntityCoroutine();
    }

    protected override IEnumerator RunEntity()
    {
        if (IsDead) yield break;

        _agent.enabled = true;
        _agent.avoidancePriority = Random.Range(1, 100);

        Vector3 pos = _spire.transform.position;
        pos.y = transform.position.y;
        while (!_agent.SetDestination(pos)) yield return null;
        while (_agent.pathPending) yield return null;
        
        Animator.SetBool("Move", true);
    }
}
