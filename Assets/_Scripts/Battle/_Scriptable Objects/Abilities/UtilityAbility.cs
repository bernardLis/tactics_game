using UnityEngine;
using System.Threading.Tasks;

public enum UtilityType { Key }

[CreateAssetMenu(menuName = "ScriptableObject/Abilities/Utility Ability")]
public class UtilityAbility : Ability
{
    [Header("Utility Ability")]
    public UtilityType utilityType;
    UtilityTriggerable utilityTriggerable;

    public override void Initialize(GameObject obj)
    {
        base.Initialize(obj);
        utilityTriggerable = obj.GetComponent<UtilityTriggerable>();
    }

    public async override Task<bool> TriggerAbility(GameObject _target)
    {
        Debug.Log("target in trigger: " + _target);

        // check if target is valid
        var itemUsableObject = _target.GetComponent<IItemUsable<UtilityAbility>>();
        Debug.Log("itemUsableObject in trigger ability: " + itemUsableObject);
        if (itemUsableObject == null)
            return false;

        // heal target if successful play sound and retrun true;
        if (!await utilityTriggerable.TriggerUtility(_target, this, characterGameObject))
            return false;

        await base.TriggerAbility(_target);
        return true;
    }

}
