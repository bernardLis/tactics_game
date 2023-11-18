using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleProjectileOpponentQuick : BattleProjectileOpponent
{
    public override void Shoot(BattleBoss boss, Vector3 dir, float time, int power)
    {
        base.Shoot(boss, dir, time, power);
        StartCoroutine(SpeedChange());
    }

    IEnumerator SpeedChange()
    {
        _speed = 4;
        yield return DOTween.To(x => _speed = x, _speed, 0, 2f).SetEase(Ease.Linear).WaitForCompletion();
        yield return new WaitForSeconds(1f);
        _speed = 15;
    }

}
