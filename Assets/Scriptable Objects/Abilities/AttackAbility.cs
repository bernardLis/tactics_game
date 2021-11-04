using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Attack Ability")]
public class AttackAbility : Ability
{
    AttackTriggerable attackTriggerable;

    public override void Initialize(GameObject obj)
    {
        base.Initialize(obj);
        attackTriggerable = obj.GetComponent<AttackTriggerable>();
    }
    
    // returns true if ability was triggered with success
    public override bool TriggerAbility(GameObject target)
    {
        if (!attackTriggerable.Attack(target, value, manaCost))
            return false;

        audioSource.clip = aSound;
        audioSource.Play();
        return true;
    }


}
