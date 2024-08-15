using DG.Tweening;
using UnityEngine;

namespace Lis.Camp.Building
{
    public class CrateController : MonoBehaviour
    {
        void Start()
        {
            transform.DOScale(1.3f, 0.5f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }

        public void DestroySelf()
        {
            transform.DOKill();
            transform.DOMoveY(1f, 0.5f).SetEase(Ease.InOutSine);
            transform.DOScale(0f, 0.5f).SetEase(Ease.OutBack)
                .SetDelay(0.5f)
                .OnComplete(() => Destroy(gameObject));
        }
    }
}