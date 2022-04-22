using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;

public enum UtilityType { Key }

[CreateAssetMenu(menuName = "ScriptableObject/Abilities/Utility Ability")]
public class UtilityAbility : Ability
{
    [Header("Utility Ability")]
    public UtilityType UtilityType;
    UtilityTriggerable _utilityTriggerable;

    public override void Create(Dictionary<string, object> item, StatModifier statModifier, Status status)
    {
        base.Create(item, statModifier, status);
        UtilityType = (UtilityType)System.Enum.Parse(typeof(UtilityType), item["UtilityType"].ToString());
    }

    public override void Initialize(GameObject obj)
    {
        base.Initialize(obj);
        _utilityTriggerable = obj.GetComponent<UtilityTriggerable>();
    }

    public async override Task AbilityLogic(Vector3 pos)
    {
        await _utilityTriggerable.TriggerUtility(pos, this, CharacterGameObject);
    }


}
