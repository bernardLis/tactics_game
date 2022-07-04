using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "ScriptableObject/Stats/Modifiers/Modifier")]
public class StatModifier : BaseScriptableObject
{
    public string ReferenceID;
    public Sprite Icon;
    public StatType StatType;
    public int NumberOfTurns;
    public int Value;

    GameObject _characterGameObject;
    ObjectUI _damageUI;

    // called from editor using table data
    public virtual void Create(Dictionary<string, object> item)
    {
        ReferenceID = item["ReferenceID"].ToString();
        StatType = (StatType)System.Enum.Parse(typeof(StatType), item["StatType"].ToString());
        NumberOfTurns = int.Parse(item["NumberOfTurns"].ToString());
        Value = int.Parse(item["Value"].ToString());
        Icon = (Sprite)AssetDatabase.LoadAssetAtPath($"Assets/Sprites/Ability/{item["Icon"]}", typeof(Sprite));
    }

    public virtual void Initialize(GameObject self)
    {
        _characterGameObject = self;
        _damageUI = self.GetComponent<ObjectUI>();

        string str = "";

        if (Value < 0)
            str = $"{Value} {StatType.ToString()}";
        if (Value > 0)
            str = $"+{Value} {StatType.ToString()}";

        _damageUI.DisplayOnCharacter(str, 24, Color.white);
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
