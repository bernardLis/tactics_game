using DG.Tweening;
using UnityEngine;

namespace Lis.Battle.Tiles
{
    public class BorderController : MonoBehaviour
    {

        public void EnableBorder()
        {
            float y = transform.localScale.y;
            transform.localScale = new Vector3(transform.localScale.x, 0, transform.localScale.z);
            transform.DOScaleY(y, 0.5f)
                .SetEase(Ease.InOutSine);
        }

        public void DestroySelf()
        {
            if (this == null) return;

            transform.DOScaleY(0, 0.5f)
                .SetEase(Ease.InOutSine)
                .OnComplete(() => Destroy(gameObject));
        }

    }
}
