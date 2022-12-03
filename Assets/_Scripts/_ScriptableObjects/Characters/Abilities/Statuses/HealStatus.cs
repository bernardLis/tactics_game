using UnityEngine;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "ScriptableObject/Character/Ability/Statuses/Heal")]
public class HealStatus : Status
{
    public override async Task TriggerStatus()
    {
        await base.TriggerStatus();
        if (_characterStats == null)
            return;
        _characterStats.GainHealth(Value, _characterGameObject, null);
    }

    public override string GetDescription()
    {
        return "Heal " + Value + " for " + NumberOfTurns + " turn/s.";
    }
}
