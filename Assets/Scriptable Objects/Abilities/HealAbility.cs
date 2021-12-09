using UnityEngine;
using System.Threading.Tasks;

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
    public async override Task<bool> TriggerAbility(GameObject _target)
    {
        // check if target is valid
        var healableObject = _target.GetComponent<IHealable<Ability>>();
        if (healableObject == null)
            return false;

        // heal target if successful play sound and retrun true;
        if (!await healTriggerable.Heal(_target, this))
            return false;

        await base.TriggerAbility(_target);
        return true;
    }

}
