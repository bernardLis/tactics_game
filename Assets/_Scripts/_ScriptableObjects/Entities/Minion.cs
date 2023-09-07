using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Minion")]
public class Minion : EntityMovement
{
    public override void InitializeBattle(int team)
    {
        base.InitializeBattle(team);
        if (EntityName.Length == 0) EntityName = Helpers.ParseScriptableObjectName(name);

        if (Level.Value <= 1) return;

        for (int i = 1; i < Level.Value; i++)
        {
            MaxHealth.LevelUp();
            Armor.LevelUp();
            Speed.LevelUp();
        }
    }
}

