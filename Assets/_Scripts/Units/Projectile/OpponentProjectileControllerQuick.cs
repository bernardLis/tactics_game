using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Lis.Units.Projectile
{
    public class OpponentProjectileControllerQuick : OpponentProjectileController
    {
        public override void Shoot(Vector3 dir, float time)
        {
            base.Shoot(dir, time);
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