using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Experimental.GlobalIllumination;

public class BattleSpireLight : MonoBehaviour
{
    [SerializeField] Light _pointLight;

    float _pointLightIntensity;
    void Start()
    {
        transform.DOLocalMoveY(5f, Random.Range(3f, 5f)).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        _pointLightIntensity = _pointLight.intensity;
    }

    public void PauseLight()
    {
        transform.DOPause();
        DOTween.To(x => _pointLight.intensity = x, _pointLight.range, 0, 1f)
                .SetLoops(-1, LoopType.Yoyo).SetId("bla");
    }



    public void ResumeLight()
    {
        DOTween.Pause("bla");
        _pointLight.intensity = _pointLightIntensity;
        transform.DORestart();
    }

}
