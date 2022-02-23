using UnityEngine;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "ScriptableObject/Abilities/Buff Ability")]
public class BuffAbility : Ability
{
    BuffTriggerable _buffTriggerable;

    public override void Initialize(GameObject obj)
    {
        base.Initialize(obj);
        _buffTriggerable = obj.GetComponent<BuffTriggerable>();
    }

    // returns true if ability was triggered with success
    public async override Task<bool> TriggerAbility(GameObject target)
    {
        // check if target is valid
        var stats = target.GetComponent<CharacterStats>();
        if (stats == null)
            return false;

        // interact
        if (!await _buffTriggerable.Buff(target, this, _characterGameObject))
            return false;

        await base.TriggerAbility(target);
        return true;
    }

}
