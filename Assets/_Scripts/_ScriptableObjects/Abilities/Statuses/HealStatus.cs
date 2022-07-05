using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Statuses/Heal")]
public class HealStatus : Status
{
    public override void TriggerStatus()
    {
        base.TriggerStatus();
        if (_characterStats == null)
            return;
        _characterStats.GainHealth(Value, _characterGameObject, null);
    }

    public override string GetDescription()
    {
        return "Heal " + Value + " for " + NumberOfTurns + " turn/s.";
    }
}
