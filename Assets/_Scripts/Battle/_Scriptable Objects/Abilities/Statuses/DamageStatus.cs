using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Statuses/Damage")]
public class DamageStatus : Status
{
    public override void TriggerStatus()
    {
        base.TriggerStatus();
        if (_characterStats != null)
            _characterStats.TakeDamageNoDodgeNoRetaliation(Value).GetAwaiter();
    }

    public override string GetDescription()
    {
        return "Damage " + Value + " for " + NumberOfTurns + " turn/s.";
    }

}
