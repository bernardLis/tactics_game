using UnityEngine;

[CreateAssetMenu(menuName = "Statuses/Heal")]
public class StatusHeal : Status
{
    CharacterStats characterStats;
    public override void Initialize(GameObject _self)
    {
        base.Initialize(_self);

        characterStats = _self.GetComponent<CharacterStats>();
    }

    public override void TriggerStatus()
    {
        base.TriggerStatus();
        characterStats.GainHealth(value, null);
    }

    public override string GetDescription()
    {
        return "Heals target by " + value + " for " + numberOfTurns + " turn/s.";
    }
}
