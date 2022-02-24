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

    // called from editor using table data
    public virtual void Create(Dictionary<string, object> item)
    {
        ReferenceID = item["ReferenceID"].ToString();
        Icon = (Sprite)AssetDatabase.LoadAssetAtPath($"Assets/Sprites/Ability/StatModifier/{item["Icon"]}", typeof(Sprite));
        StatType = (StatType)System.Enum.Parse(typeof(StatType), item["StatType"].ToString());
        NumberOfTurns = int.Parse(item["NumberOfTurns"].ToString());
        Value = int.Parse(item["Value"].ToString());
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
