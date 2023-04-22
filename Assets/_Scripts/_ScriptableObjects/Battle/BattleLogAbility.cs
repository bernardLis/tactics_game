using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleLogAbility : BattleLog
{

    public Ability Ability;
    public int NumberOfAffectedEntities;
    public int DamageDealt;

    public void Initialize(Ability ability, int numberOfAffectedEntities, int damageDealt)
    {
        SetTime();
        Ability = ability;
        NumberOfAffectedEntities = numberOfAffectedEntities;
        DamageDealt = damageDealt;
    }

}
