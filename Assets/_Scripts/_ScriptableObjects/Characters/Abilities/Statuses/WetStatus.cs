using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "ScriptableObject/Character/Ability/Statuses/Wet")]
public class WetStatus : Status
{
    public override async Task FirstTrigger()
    {
        await base.FirstTrigger();
        await Task.Yield();
    }

    public override void AddFlag()
    {
        if (_baseStats == null)
            return;

        _baseStats.SetIsWet(true);
    }

    public override void ResetFlag()
    {
        if (_baseStats == null)
            return;

        _baseStats.SetIsWet(false);
    }

    public override string GetDescription()
    {
        return "Wet for " + NumberOfTurns + " turn/s. Blocks burn status once. Doubles electrification status damage.";
    }

}
