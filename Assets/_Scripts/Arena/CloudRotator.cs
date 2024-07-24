using DG.Tweening;
using UnityEngine;

namespace Lis.Arena
{
    public class CloudRotator : MonoBehaviour
    {
        void Start()
        {
            transform.DORotate(new(0, 360, 0), 240f, RotateMode.FastBeyond360)
                .SetLoops(-1);
        }
    }
}