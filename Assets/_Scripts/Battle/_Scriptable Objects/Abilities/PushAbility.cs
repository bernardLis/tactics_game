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

    public async override Task AbilityLogic(Vector3 pos)
    {
        // push if successful play sound and retrun true;
        await _pushTriggerable.Push(pos, this);
    }
}
