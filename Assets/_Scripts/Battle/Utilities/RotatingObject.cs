using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RotatingObject : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.DORotate(new Vector3(0, 0, 360), 4f, RotateMode.LocalAxisAdd).SetLoops(-1, LoopType.Incremental);
    }
}
