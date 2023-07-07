using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BattleHighlightDiamond : MonoBehaviour
{

    Material _mat;
    Vector3 _startPos;

    public void Enable(Color col)
    {
        if (gameObject.activeSelf) return;

        _startPos = transform.localPosition;
        gameObject.SetActive(true);

        if (_mat == null) _mat = GetComponentInChildren<MeshRenderer>().material;
        _mat.color = col;

        float endY = transform.localPosition.y + 0.5f;
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
        transform.localPosition = _startPos;
        gameObject.SetActive(false);

    }
}
