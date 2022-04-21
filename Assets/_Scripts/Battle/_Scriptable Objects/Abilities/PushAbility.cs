using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "ScriptableObject/Abilities/Push Ability")]
public class PushAbility : Ability
{
    PushTriggerable _pushTriggerable;

    public override void Initialize(GameObject obj)
    {
        base.Initialize(obj);
        _pushTriggerable = obj.GetComponent<PushTriggerable>();
    }

    public async override Task AbilityLogic(GameObject target)
    {
        // check if target is valid
        var pushableObject = target.GetComponent<IPushable<Vector3, GameObject, Ability>>();
        if (pushableObject == null)
            return;

        // push if successful play sound and retrun true;
        await _pushTriggerable.Push(target, this);
    }
}
