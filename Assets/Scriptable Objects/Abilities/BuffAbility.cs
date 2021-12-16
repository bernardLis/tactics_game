using UnityEngine;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "Abilities/Buff Ability")]
public class BuffAbility : Ability
{
    BuffTriggerable buffTriggerable;

    public override void Initialize(GameObject obj)
    {
        base.Initialize(obj);
        buffTriggerable = obj.GetComponent<BuffTriggerable>();
    }

    // returns true if ability was triggered with success
    public async override Task<bool> TriggerAbility(GameObject _target)
    {
        // check if target is valid
        var stats = _target.GetComponent<CharacterStats>();
        if (stats == null)
            return false;

        // interact
        if (!await buffTriggerable.Buff(_target, this, characterGameObject))
            return false;

        await base.TriggerAbility(_target);
        return true;
    }

}
