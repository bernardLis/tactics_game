using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Statuses/Mana")]
public class ManaStatus : Status
{
    public override void TriggerStatus()
    {
        base.TriggerStatus();
        if (_characterStats == null)
            return;
        _characterStats.GainMana(Value);
    }

    public override string GetDescription()
    {
        return "Gain " + Value + " mana for " + NumberOfTurns + " turn/s.";
    }
}
