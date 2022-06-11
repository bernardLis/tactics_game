using UnityEngine;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class LightManager : Singleton<LightManager>
{
    Light2D _globalLight;
    float _initialIntensity;
    Color _initialColor;

    public void Initialize(Light2D light)
    {
        _globalLight = light;
        _initialIntensity = _globalLight.intensity;
        _initialColor = _globalLight.color;
    }

    public void ChangeGlobalLightIntensity(float value, float duration)
    {
        DOTween.To(() => _globalLight.intensity, x => _globalLight.intensity = x, value, duration);
    }

    public void FlashGlobalLight(float value, float duration, int loops)
    {
        DOTween.To(() => _globalLight.intensity, x => _globalLight.intensity = x, value, duration)
                    .SetLoops(loops, LoopType.Yoyo);
    }

    public void ResetGlobalLightIntensity(float duration)
    {
        DOTween.To(() => _globalLight.intensity, x => _globalLight.intensity = x, _initialIntensity, duration);
    }

    public void ChangeGlobalLightColor(Color color, float duration)
    {
        DOTween.To(() => _globalLight.color, x => _globalLight.color = x, color, duration);
    }

    public void ResetGlobalLightColor(float duration)
    {
        DOTween.To(() => _globalLight.color, x => _globalLight.color = x, _initialColor, duration);
    }


}
