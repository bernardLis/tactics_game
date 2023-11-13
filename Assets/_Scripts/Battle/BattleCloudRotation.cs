using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleCloudRotation : MonoBehaviour
{
    void Start()
    {
        transform.DORotate(new Vector3(0, 360, 0), 240f, RotateMode.FastBeyond360)
                 .SetLoops(-1);
    }

}
