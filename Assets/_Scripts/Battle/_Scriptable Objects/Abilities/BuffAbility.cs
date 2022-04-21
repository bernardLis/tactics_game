using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "ScriptableObject/Abilities/Buff Ability")]
public class BuffAbility : Ability
{
    BuffTriggerable _buffTriggerable;

    public override void Initialize(GameObject obj)
    {
        base.Initialize(obj);
        _buffTriggerable = obj.GetComponent<BuffTriggerable>();
    }

    public async override Task AbilityLogic(GameObject target)
    {
        // check if target is valid
        var stats = target.GetComponent<CharacterStats>();
        if (stats == null)
            return;

        // interact
        await _buffTriggerable.Buff(target, this, CharacterGameObject);
    }

}
