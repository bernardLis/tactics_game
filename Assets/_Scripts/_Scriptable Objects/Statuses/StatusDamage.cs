using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Statuses/Damage")]
public class StatusDamage : Status
{
    CharacterStats characterStats;
    public override void Initialize(GameObject _self, GameObject _attacker)
    {
        base.Initialize(_self, _attacker);

        characterStats = _self.GetComponent<CharacterStats>();
    }

    public override void TriggerStatus()
    {
        base.TriggerStatus();
        characterStats.TakeDamageNoDodgeNoRetaliation(value);
    }

    public override string GetDescription()
    {
        return "Damages target by " + value + " for " + numberOfTurns + " turn/s.";
    }

}
