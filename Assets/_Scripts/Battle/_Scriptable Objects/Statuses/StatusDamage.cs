using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Statuses/Damage")]
public class StatusDamage : Status
{
    public override void TriggerStatus()
    {
        base.TriggerStatus();
        _characterStats.TakeDamageNoDodgeNoRetaliation(Value);
    }

    public override string GetDescription()
    {
        return "Damages target by " + Value + " for " + NumberOfTurns + " turn/s.";
    }

}
