using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMinion : BattleCreatureMelee
{
    [Header("Minion")]
    [SerializeField] Sound _reachedSpireSound;
    public Minion Minion { get; private set; }

    BattleHero _targetHero;

    public override void InitializeEntity(Entity entity)
    {
        base.InitializeEntity(entity);
        Minion = (Minion)entity;
    }

    public override void InitializeBattle(int team, ref List<BattleEntity> opponents)
    {
        base.InitializeBattle(team, ref opponents);

        _targetHero = _battleManager.GetComponent<BattleHeroManager>().BattleHero;

        StartRunEntityCoroutine();
    }

    protected override IEnumerator RunEntity()
    {
        if (IsDead) yield break;
        SetOpponent(_targetHero);

        Vector3 pos = _targetHero.transform.position;
        pos.y = transform.position.y;
        yield return PathToOpponent();
    }

    public void ReachedSpire()
    {
        _audioManager.PlaySFX(_reachedSpireSound, transform.position);
    }
}
