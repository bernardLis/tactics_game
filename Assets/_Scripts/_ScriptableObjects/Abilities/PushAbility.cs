using UnityEngine;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "ScriptableObject/Abilities/Push Ability")]
public class PushAbility : Ability
{
    PushTriggerable _pushTriggerable;

    public override void Initialize(GameObject obj)
    {
        base.Initialize(obj);
        _pushTriggerable = obj.GetComponent<PushTriggerable>();
    }

    public override bool IsTargetViable(GameObject target)
    {
        // you can push allies but they are not returned here
        if (target.TryGetComponent(out IPushable<Vector3, GameObject, Ability> pushable)
            && !target.CompareTag(CharacterGameObject.tag))
            return true;
        return false;
    }

    public async override Task AbilityLogic(Vector3 pos)
    {
        // push if successful play sound and retrun true;
        _stats.Character.GetExp(10);
        await _pushTriggerable.Push(pos, this);
    }
}
