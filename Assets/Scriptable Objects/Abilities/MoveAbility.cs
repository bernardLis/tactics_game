using UnityEngine;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "Abilities/Move Ability")]
public class MoveAbility : Ability
{
    PushTriggerable pushTriggerable;

    public override void Initialize(GameObject obj)
    {
        base.Initialize(obj);
        pushTriggerable = obj.GetComponent<PushTriggerable>();
    }

    // returns true if ability was triggered with success
    public async override Task<bool> TriggerAbility(GameObject target)
    {
        if (!pushTriggerable.Push(target, manaCost))
            return false;

        audioSource.clip = aSound;
        audioSource.Play();
        return true;
    }
}
