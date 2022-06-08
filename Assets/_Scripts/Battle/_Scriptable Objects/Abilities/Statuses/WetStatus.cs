using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "ScriptableObject/Statuses/Wet")]
public class WetStatus : Status
{
    public override async void FirstTrigger()
    {
        base.FirstTrigger();

        await Task.Yield();
    }

    public override void AddFlag()
    {
        _characterStats.SetIsWet(true);
    }

    public override void ResetFlag()
    {
        _characterStats.SetIsWet(false);
    }

    public override string GetDescription()
    {
        return "Wet for " + NumberOfTurns + " turn/s. Blocks burn status once. Doubles electrification status damage.";
    }

}
