using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "ScriptableObject/Character/Ability/Statuses/Stat Modifier")]
public class StatModifier : BaseScriptableObject
{
    public string ReferenceID;
    public Sprite Icon;
    public StatType StatType;
    public int NumberOfTurns;
    public int Value;

    GameObject _characterGameObject;
    ObjectUI _objectUI;

    public virtual void Initialize(GameObject self)
    {
        _characterGameObject = self;
        _objectUI = self.GetComponent<ObjectUI>();

        string str = "";

        if (Value < 0)
            str = $"{Value} {StatType.ToString()}";
        if (Value > 0)
            str = $"+{Value} {StatType.ToString()}";

        _objectUI.DisplayOnCharacter(str, 24, Color.white);
    }

    public string GetDescription()
    {
        string str = "";

        if (Value < 0)
            str += "Decrease ";
        if (Value > 0)
            str += "Increase ";

        str += StatType + " by " + Mathf.Abs(Value) + " for " + NumberOfTurns + " turn/s.";

        return str;
    }
}
