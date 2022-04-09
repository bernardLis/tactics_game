using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Statuses/Damage")]
public class DamageStatus : Status
{
    public override void TriggerStatus()
    {
        base.TriggerStatus();
        _characterStats.TakeDamageNoDodgeNoRetaliation(Value);
    }

    public override string GetDescription()
    {
        return "Damage " + Value + " for " + NumberOfTurns + " turn/s.";
    }

}
