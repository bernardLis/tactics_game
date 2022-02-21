using UnityEngine;
using DG.Tweening;

public class SelectionArrow : MonoBehaviour
{
    Vector3 startPositon;
    Vector3 endPositon = new Vector3(0f, 1.5f, 0f);
    Vector3 startScale;
    Vector3 endScale = new Vector3(0.35f, 0.35f, 0.35f);

    public void Start()
    {
        startPositon = transform.localPosition;
        startScale = transform.localScale;
    }
    void OnEnable()
    {
        transform.DOScale(endScale, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        transform.DOLocalMove(endPositon, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }

    void OnDisable()
    {
        // reseting scale and position;
        transform.localPosition = startPositon;
        transform.localScale = startScale;

        DOTween.Kill(transform);
    }
}
