using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "ScriptableObject/Abilities/Heal Ability")]
public class HealAbility : Ability
{

    HealTriggerable _healTriggerable;

    public override void Initialize(GameObject obj)
    {
        base.Initialize(obj);
        _healTriggerable = obj.GetComponent<HealTriggerable>();
    }

    public async override Task AbilityLogic(GameObject target)
    {
        // check if target is valid
        var healableObject = target.GetComponent<IHealable<GameObject, Ability>>();
        if (healableObject == null)
            return;

        // heal target if successful play sound and retrun true;
        await _healTriggerable.Heal(target, this, CharacterGameObject);
    }

    public override int CalculateInteractionResult(CharacterStats attacker, CharacterStats defender)
    {
        // return positive value coz it is heal // TODO: this is not a perfect way to do it...
        return -1 * base.CalculateInteractionResult(attacker, defender);
    }

}
