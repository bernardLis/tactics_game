using UnityEngine;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "Abilities/Push Ability")]
public class PushAbility : Ability
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
        // check if target is valid
        var pushableObject = target.GetComponent<IPushable<Vector3>>();
        if (pushableObject == null)
            return false;

        // highlight only target
        await Highlighter.instance.ClearHighlightedTiles();
        Highlighter.instance.HighlightSingle(target.transform.position, highlightColor);

        // push if successful play sound and retrun true;
        if (!await pushTriggerable.Push(target, manaCost))
            return false;

        audioSource.clip = aSound;
        audioSource.Play();
        return true;
    }
}
