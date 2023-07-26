using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMinion : BattleEntity
{
    public Minion Minion { get; private set; }

    public override void InitializeEntity(Entity entity)
    {
        base.InitializeEntity(entity);
        Minion = (Minion)entity;
    }

    public override void InitializeBattle(int team, ref List<BattleEntity> opponents)
    {
        base.InitializeBattle(team, ref opponents);

        StartRunEntityCoroutine();
    }

    protected override IEnumerator RunEntity()
    {
        if (IsDead) yield break;

        _agent.enabled = true;
        _agent.avoidancePriority = Random.Range(1, 100);

        // HERE: get spire position
        while (!_agent.SetDestination(Vector3.zero)) yield return null;
        Animator.SetBool("Move", true);
        while (_agent.pathPending) yield return null;
    }
}
