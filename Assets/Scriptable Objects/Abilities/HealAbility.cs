using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Heal Ability")]
public class HealAbility : Ability
{

    HealTriggerable healTriggerable;

    public override void Initialize(GameObject obj)
    {
        base.Initialize(obj);
        healTriggerable = obj.GetComponent<HealTriggerable>();
    }

    // returns true if ability was triggered with success
    public override bool TriggerAbility(GameObject target)
    {
        if (!healTriggerable.Heal(target, value, manaCost))
            return false;

        audioSource.clip = aSound;
        audioSource.Play();
        return true;
    }

}
