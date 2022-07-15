using UnityEngine;
using System.Threading.Tasks;

public class ThunderBoltEffect : AbilityEffect
{
    public override async Task Play(Ability ability, Vector3 targetPos)
    {
        LightManager lightManager = LightManager.Instance;
        lightManager.ChangeGlobalLightIntensity(0.25f, 0.5f);
        await Task.Delay(500);

        // flash the light twice
        lightManager.FlashGlobalLight(10, 0.05f, 4);
        await Task.Delay(300);

        // TODO: spawn the effect that emits light << nope? 

        // get light back to normal 
        ElectricLineController controller = GetComponent<ElectricLineController>();
        controller.Electrify(targetPos);
        Vector3 skyPos = new Vector3(targetPos.x + Random.Range(1f, 2f), targetPos.y + 20f);
        controller.AddPosition(skyPos);
        await Task.Delay(300);
        lightManager.ResetGlobalLightIntensity(0.5f);
        await Task.Delay(500);

        DestroySelf();
    }
}
