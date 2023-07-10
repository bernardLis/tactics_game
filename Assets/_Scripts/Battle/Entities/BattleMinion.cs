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
        // HERE: MINION IMPLEMENT
        yield return null;
    }
}
