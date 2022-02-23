using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Stats/Modifiers/Modifier")]
public class StatModifier : BaseScriptableObject
{
    public StatType StatType;
    public int NumberOfTurns;
    public int Value;
    public Sprite Icon;

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
