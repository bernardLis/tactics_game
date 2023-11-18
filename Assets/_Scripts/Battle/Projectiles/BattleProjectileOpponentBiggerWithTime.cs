using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleProjectileOpponentBiggerWithTime : BattleProjectileOpponent
{
    public override void Shoot(BattleBoss boss, Vector3 dir, float time, int power)
    {
        base.Shoot(boss, dir, time, power);

    }

}
