using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Lis
{
    public class BattleProjectileOpponentBiggerWithTime : BattleProjectileOpponent
    {
        public override void Shoot(BattleEntity entity, Vector3 dir, float time, int power)
        {
            transform.localScale = Vector3.one * 0.5f;

            base.Shoot(entity, dir, time, power);

            float scale = 1;
            float scaleMultiplier = 3;
            Transform t = transform;
            t.DOScale(scale * 3, time).SetEase(Ease.InOutSine);
            float currentY = t.position.y;
            transform.DOMoveY(currentY + scaleMultiplier * 0.25f, time).SetEase(Ease.InOutSine);
        }

        protected override IEnumerator Explode(Vector3 position)
        {
            yield return base.Explode(position);
            transform.localScale = Vector3.one * 0.5f;
        }
    }
}