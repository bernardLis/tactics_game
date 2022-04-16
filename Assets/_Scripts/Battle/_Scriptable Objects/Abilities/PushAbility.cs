using UnityEngine;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "ScriptableObject/Abilities/Push Ability")]
public class PushAbility : Ability
{
    PushTriggerable _pushTriggerable;

    public override void Initialize(GameObject obj)
    {
        base.Initialize(obj);
        _pushTriggerable = obj.GetComponent<PushTriggerable>();
    }

    // returns true if ability was triggered with success
    public async override Task<bool> TriggerAbility(GameObject target)
    {
        await base.TriggerAbility(target);

        // check if target is valid
        var pushableObject = target.GetComponent<IPushable<Vector3, GameObject, Ability>>();
        if (pushableObject == null)
            return false;

        // push if successful play sound and retrun true;
        if (!await _pushTriggerable.Push(target, this))
            return false;

        return true;
    }
}
