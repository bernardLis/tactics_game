using UnityEngine;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "ScriptableObject/Ability/Statuses/Damage")]
public class DamageStatus : Status
{
    public async override Task TriggerStatus()
    {
        await base.TriggerStatus();
        if (_characterStats != null)
            await _characterStats.TakeDamageFinal(Value);
        await Task.Yield();
    }

    public override string GetDescription()
    {
        return "Damage " + Value + " for " + NumberOfTurns + " turn/s.";
    }

}
