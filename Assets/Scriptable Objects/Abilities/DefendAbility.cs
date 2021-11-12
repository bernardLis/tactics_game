using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Defend Ability")]
public class DefendAbility : Ability
{
    DefendTriggerable defendTriggerable;

    public override void Initialize(GameObject obj)
    {
        base.Initialize(obj);
        defendTriggerable = obj.GetComponent<DefendTriggerable>();
    }
    
    // returns true if ability was triggered with success
    public override bool TriggerAbility(GameObject target)
    {
        Debug.Log("trigger ability is called");
        if (!defendTriggerable.Defend(target, value, manaCost))
            return false;

        audioSource.clip = aSound;
        audioSource.Play();
        return true;
    }


}
