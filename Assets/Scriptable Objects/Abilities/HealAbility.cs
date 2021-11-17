using UnityEngine;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "Abilities/Heal Ability")]
public class HealAbility : Ability
{

    HealTriggerable healTriggerable;

    public override void Initialize(GameObject obj)
    {
        base.Initialize(obj);
        healTriggerable = obj.GetComponent<HealTriggerable>();
    }

    // returns true if ability was triggered with success
    public async override Task<bool> TriggerAbility(GameObject target)
    {
        // check if target is valid
        var healableObject = target.GetComponent<IHealable>();
        if (healableObject == null)
            return false;

        // highlight only target
        await Highlighter.instance.ClearHighlightedTiles();
        Highlighter.instance.HighlightSingle(target.transform.position, highlightColor);

        // heal target if successful play sound and retrun true;
        if (!await healTriggerable.Heal(target, value, manaCost))
            return false;

        audioSource.clip = aSound;
        audioSource.Play();
        return true;
    }

}
