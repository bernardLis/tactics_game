using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleHighlightDiamond : MonoBehaviour
{

    Material _mat;

    void Start()
    {
        Disable();
        _mat = GetComponentInChildren<MeshRenderer>().material;
    }

    public void Enable(Color col)
    {
        gameObject.SetActive(true);
        _mat.color = col;

        float endY = transform.position.y + 0.5f;
        transform.DOMoveY(endY, 2f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);

        transform.DORotate(new Vector3(0, 360, 0), 5f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);
    }

    public void Disable()
    {
        DOTween.Kill(transform);
        gameObject.SetActive(false);

    }
}
