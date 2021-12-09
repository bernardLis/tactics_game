using UnityEngine;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "Abilities/Push Ability")]
public class PushAbility : Ability
{
    PushTriggerable pushTriggerable;

    public override void Initialize(GameObject obj)
    {
        base.Initialize(obj);
        pushTriggerable = obj.GetComponent<PushTriggerable>();
    }

    // returns true if ability was triggered with success
    public async override Task<bool> TriggerAbility(GameObject _target)
    {
        // check if target is valid
        var pushableObject = _target.GetComponent<IPushable<Vector3, Ability>>();
        if (pushableObject == null)
            return false;

        // push if successful play sound and retrun true;
        if (!await pushTriggerable.Push(_target, this))
            return false;

        await base.TriggerAbility(_target);
        return true;
    }
}
