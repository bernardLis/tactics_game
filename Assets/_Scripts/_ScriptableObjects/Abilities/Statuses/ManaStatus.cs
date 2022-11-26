using UnityEngine;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "ScriptableObject/Ability/Statuses/Mana")]
public class ManaStatus : Status
{
    public override async Task TriggerStatus()
    {
        await base.TriggerStatus();
        if (_characterStats == null)
            return;
        _characterStats.GainMana(Value);
    }

    public override string GetDescription()
    {
        return "Gain " + Value + " mana for " + NumberOfTurns + " turn/s.";
    }
}
