using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Lis.Units.Projectile
{
    public class OpponentProjectileControllerQuick : OpponentProjectileController
    {
        public override void Shoot(UnitController entity, Vector3 dir, float time, int power)
        {
            base.Shoot(entity, dir, time, power);
            StartCoroutine(SpeedChange());
        }

        IEnumerator SpeedChange()
        {
            Speed = 4;
            yield return DOTween.To(x => Speed = x, Speed, 0, 2f).SetEase(Ease.Linear).WaitForCompletion();
            yield return new WaitForSeconds(1f);
            Speed = 15;
        }

    }
}
