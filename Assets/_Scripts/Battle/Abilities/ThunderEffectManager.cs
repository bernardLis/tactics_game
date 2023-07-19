using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class ThunderEffectManager : MonoBehaviour
{
    [SerializeField] Transform _cloud;
    void Start()
    {
        _cloud.DOMoveY(0f, 0.2f).SetEase(Ease.OutFlash)
            .OnComplete(() => _cloud.DOScale(2f, 0.2f).SetEase(Ease.OutFlash)
                .OnComplete(() => Destroy(_cloud.gameObject)));
    }

}
