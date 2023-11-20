using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleProjectileOpponentQuick : BattleProjectileOpponent
{
    public override void Shoot(BattleEntity entity, Vector3 dir, float time, int power)
    {
        base.Shoot(entity, dir, time, power);
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
