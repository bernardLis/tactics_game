using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class BattleFireFXController : MonoBehaviour
{

    public void Activate()
    {
        ParticleSystem particleSystem = GetComponent<ParticleSystem>();
        var emission = particleSystem.emission;
        emission.rateOverTime = 0f;

        DOTween.To(x => emission.rateOverTime = x, 0, 12, 2f);
    }

    public void Deactivate()
    {
        ParticleSystem particleSystem = GetComponent<ParticleSystem>();
        var emission = particleSystem.emission;
        float count = emission.rateOverTime.constant;

        DOTween.To(x => emission.rateOverTime = x, count, 0, 2f);
    }
}
