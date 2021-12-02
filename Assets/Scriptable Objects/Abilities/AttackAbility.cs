using UnityEngine;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "Abilities/Attack Ability")]
public class AttackAbility : Ability
{
    AttackTriggerable attackTriggerable;

    public override void Initialize(GameObject obj)
    {
        base.Initialize(obj);
        attackTriggerable = obj.GetComponent<AttackTriggerable>();
    }

    // returns true if ability was triggered with success
    public async override Task<bool> TriggerAbility(GameObject target)
    {
        // check if target is valid
        var attackableObject = target.GetComponent<IAttackable<GameObject>>();
        if (attackableObject == null)
            return false;

        // highlight only target
        //await Highlighter.instance.ClearHighlightedTiles();
        //Highlighter.instance.HighlightSingle(target.transform.position, highlightColor);

        // interact
        if (!await attackTriggerable.Attack(target, value, manaCost, aProjectile))
            return false;

        // sound
        audioSource.clip = aSound;
        audioSource.Play();

        return true;
    }


}
