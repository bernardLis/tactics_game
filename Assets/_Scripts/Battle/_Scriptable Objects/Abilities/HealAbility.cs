using UnityEngine;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "ScriptableObject/Abilities/Heal Ability")]
public class HealAbility : Ability
{

    HealTriggerable _healTriggerable;

    public override void Initialize(GameObject obj)
    {
        base.Initialize(obj);
        _healTriggerable = obj.GetComponent<HealTriggerable>();
    }

    // returns true if ability was triggered with success
    public async override Task<bool> TriggerAbility(GameObject target)
    {
        // check if target is valid
        var healableObject = target.GetComponent<IHealable<GameObject, Ability>>();
        if (healableObject == null)
            return false;

        // heal target if successful play sound and retrun true;
        if (!await _healTriggerable.Heal(target, this, _characterGameObject))
            return false;

        await base.TriggerAbility(target);
        return true;
    }

    public override int CalculateInteractionResult(CharacterStats attacker, CharacterStats defender)
    {
        // return positive value coz it is heal // TODO: this is not a perfect way to do it...
        return -1 * base.CalculateInteractionResult(attacker, defender);
    }

}
