using UnityEngine;
using System.Threading.Tasks;

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
    public async override Task<bool> TriggerAbility(GameObject target)
    {
        if (!await defendTriggerable.Defend(target, value, manaCost))
            return false;

        audioSource.clip = aSound;
        audioSource.Play();
        return true;
    }


}
