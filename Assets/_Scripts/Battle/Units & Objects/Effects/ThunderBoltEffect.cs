using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class ThunderBoltEffect : Effect
{
    public override async Task Play(Ability ability, Vector3 targetPos)
    {
        BoardManager bm = BoardManager.Instance;
        Light2D globalLight = bm.GlobalLight;
        float startIntensity = globalLight.intensity;
        float targetIntensity = 0.25f;
        float flashIntensity = 10f;

        DOTween.To(() => globalLight.intensity, x => globalLight.intensity = x, targetIntensity, 0.5f);
        await Task.Delay(500);

        // flash the light twice
        DOTween.To(() => globalLight.intensity, x => globalLight.intensity = x, flashIntensity, 0.05f)
                .SetLoops(4, LoopType.Yoyo);
        await Task.Delay(300);

        // spawn the effect that emits light << nope? 

        // get light back to normal 
        ElectricLineController controller = GetComponent<ElectricLineController>();
        controller.Electrify(targetPos);
        Vector3 skyPos = new Vector3(targetPos.x + Random.Range(1f, 2f), targetPos.y + 20f);
        controller.AddPosition(skyPos);
        await Task.Delay(300);
        DOTween.To(() => globalLight.intensity, x => globalLight.intensity = x, startIntensity, 0.5f);
        await Task.Delay(500);
    }
}
