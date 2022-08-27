using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Statuses/Shield")]
public class ShieldStatus : Status
{
    public override void AddFlag()
    {
        if (_baseStats == null)
            return;

        _baseStats.SetIsShielded(true);
    }

    public override void ResetFlag()
    {
        if (_baseStats == null)
            return;

        _baseStats.SetIsShielded(false);
    }

    public override string GetDescription()
    {
        return "Shielded for " + NumberOfTurns + " turn/s.";
    }

}
