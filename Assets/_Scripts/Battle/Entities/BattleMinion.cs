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
        Vector3 pos = _spire.transform.position;
        pos.y = transform.position.y;

        yield return PathToPosition(pos);
    }
}
