using UnityEngine;
using DG.Tweening;

public class OscilateScale : MonoBehaviour
{
    [SerializeField] float _targetScaleMultiplier = 1.2f;
    [SerializeField] float _duration = 1f;
    Vector3 _startScale;
    Vector3 _endScale;
    void Start()
    {
        _startScale = transform.localScale;
        _endScale = new Vector3(_startScale.x * _targetScaleMultiplier, _startScale.y * _targetScaleMultiplier, _startScale.z * _targetScaleMultiplier);
        transform.DOScale(_endScale, _duration).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }

    public void SetOscilation(bool isOscilating)
    {
        if (isOscilating)
        {
            // TODO: potentially risky if I will be tweening other things on the same transform.
            if (DOTween.IsTweening(transform))
                return;

            transform.DOScale(_endScale, _duration).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
            return;
        }

        // reseting scale and killing the tween;
        transform.localScale = _startScale;
        DOTween.Kill(transform);
    }

}
