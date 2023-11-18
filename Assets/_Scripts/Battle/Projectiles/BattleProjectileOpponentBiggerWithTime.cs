using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleProjectileOpponentBiggerWithTime : BattleProjectileOpponent
{
    public override void Shoot(BattleBoss boss, Vector3 dir, float time, int power)
    {
        base.Shoot(boss, dir, time, power);

        float scale = transform.localScale.x;
        float scaleMultiplier = 3;
        transform.DOScale(scale * 3, time).SetEase(Ease.InOutSine);
        float currentY = transform.position.y;
        transform.DOMoveY(currentY + scaleMultiplier * 0.25f, time).SetEase(Ease.InOutSine);
    }
}
