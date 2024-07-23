using DG.Tweening;
using UnityEngine;

namespace Lis.Other
{
    public class EnemyRankingCamera : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            transform.DOMoveX(95, 20)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }
    }
}