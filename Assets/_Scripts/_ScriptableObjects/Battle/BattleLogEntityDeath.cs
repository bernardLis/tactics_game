using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleLogEntityDeath : BattleLog
{
    public BattleEntity Entity;
    public Ability KillerAbility;
    public BattleEntity KillerEntity;

    public void Initialize(BattleEntity entity, BattleEntity killerEntity = null, Ability killerAbility = null)
    {
        SetTime();
        Entity = entity;

        if (killerAbility != null)
        {
            KillerAbility = killerAbility;
            return;
        }

        KillerEntity = killerEntity;
    }



}
