using DG.Tweening;
using UnityEngine;

namespace Lis
{
    public class BattleCloudRotation : MonoBehaviour
    {
        void Start()
        {
            transform.DORotate(new Vector3(0, 360, 0), 240f, RotateMode.FastBeyond360)
                .SetLoops(-1);
        }

    }
}
