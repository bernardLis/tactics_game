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

    public override bool IsTargetViable(GameObject target)
    {
        // you can buff not friens but I am not returning them here 
        if (target.TryGetComponent(out IBuffable<GameObject, Ability> buffable)
            && target.CompareTag(CharacterGameObject.tag))
            return true;
        return false;
    }

    public async override Task AbilityLogic(Vector3 pos)
    {
        // interact
        await _buffTriggerable.Buff(pos, this, CharacterGameObject);
    }

}
