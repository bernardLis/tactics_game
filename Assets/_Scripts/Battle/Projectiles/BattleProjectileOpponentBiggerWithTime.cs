
using DG.Tweening;
using UnityEngine;

namespace Lis
{
    public class BattleProjectileOpponentBiggerWithTime : BattleProjectileOpponent
    {
        public override void Shoot(BattleEntity entity, Vector3 dir, float time, int power)
        {
            base.Shoot(entity, dir, time, power);

            float scale = transform.localScale.x;
            float scaleMultiplier = 3;
            transform.DOScale(scale * 3, time).SetEase(Ease.InOutSine);
            float currentY = transform.position.y;
            transform.DOMoveY(currentY + scaleMultiplier * 0.25f, time).SetEase(Ease.InOutSine);
        }
    }
}
