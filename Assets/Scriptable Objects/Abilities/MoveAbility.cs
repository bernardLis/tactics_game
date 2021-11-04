using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Move Ability")]
public class MoveAbility : Ability
{
    PushTriggerable pushTriggerable;

    public override void Initialize(GameObject obj)
    {
        base.Initialize(obj);
        pushTriggerable = obj.GetComponent<PushTriggerable>();
    }

    // returns true if ability was triggered with success
    public override bool TriggerAbility(GameObject target)
    {
        if (!pushTriggerable.Push(target, manaCost))
            return false;

        audioSource.clip = aSound;
        audioSource.Play();
        return true;
    }
}
