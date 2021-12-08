using UnityEngine;

[CreateAssetMenu(menuName = "Stats/Modifiers/Modifier")]
public class StatModifier : ScriptableObject
{
    public StatType statType;
    public int numberOfTurns;
    public int value;
    public Sprite icon;

    public string GetDescription()
    {
        string str = "";

        if (value < 0)
            str += "Decrease ";
        if (value > 0)
            str += "Increase ";

        str += statType + " by " + Mathf.Abs(value) + " for " + numberOfTurns + " turn/s.";

        return str;
    }
}
