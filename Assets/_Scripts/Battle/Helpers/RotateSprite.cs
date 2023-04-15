using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RotateSprite : MonoBehaviour
{
    void Start()
    {
        transform.DOLocalRotate(new Vector3(90, 0, 360), 7f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Incremental);
    }

}
