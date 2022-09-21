using UnityEngine;
using DG.Tweening;

public class SelectionArrow : MonoBehaviour
{
    Vector3 _startPositon;
    Vector3 _endPositon = new Vector3(0f, 1.5f, 0f);
    Vector3 _startScale;
    Vector3 _endScale = new Vector3(0.35f, 0.35f, 0.35f);

    public void Start()
    {
        _startPositon = transform.localPosition;
        _startScale = transform.localScale;
    }

    public void FlipArrow()
    {
        if (transform.rotation.eulerAngles.x != 0)
            transform.DOLocalRotate(new Vector3(0, 0, 0), 0.4f);
        else
            transform.DOLocalRotate(new Vector3(180, 0, 0), 0.4f);
    }

    void OnEnable()
    {
        transform.DOScale(_endScale, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        transform.DOLocalMove(_endPositon, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }

    void OnDisable()
    {
        // reseting scale and position;
        transform.localPosition = _startPositon;
        transform.localScale = _startScale;

        DOTween.Kill(transform);
    }
}
