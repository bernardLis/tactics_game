using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Statuses/Heal")]
public class HealStatus : Status
{
    public override void TriggerStatus()
    {
        base.TriggerStatus();
        _characterStats.GainHealth(Value, null);
    }

    public override string GetDescription()
    {
        return "Heals target by " + Value + " for " + NumberOfTurns + " turn/s.";
    }
}
