using UnityEngine;
using DG.Tweening;

public class OscilateScale : MonoBehaviour
{
    public float targetScaleMultiplier = 1.2f;
    public float duration = 1f;
    Vector3 startScale;
    Vector3 endScale;
    void Start()
    {
        startScale = transform.localScale;
        endScale = new Vector3(startScale.x * targetScaleMultiplier, startScale.y * targetScaleMultiplier, startScale.z * targetScaleMultiplier);
        transform.DOScale(endScale, duration).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }

    public void SetOscilation(bool isOscilating)
    {
        if (isOscilating)
        {
            // TODO: potentially risky if I will be tweening other things on the same transform.
            if (DOTween.IsTweening(transform))
                return;

            transform.DOScale(endScale, duration).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
            return;
        }

        // reseting scale and killing the tween;
        transform.localScale = startScale;
        DOTween.Kill(transform);
    }

}
