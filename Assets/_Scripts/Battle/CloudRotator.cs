using DG.Tweening;
using UnityEngine;

namespace Lis.Battle
{
    public class CloudRotator : MonoBehaviour
    {
        void Start()
        {
            transform.DORotate(new Vector3(0, 360, 0), 240f, RotateMode.FastBeyond360)
                .SetLoops(-1);
        }

    }
}
