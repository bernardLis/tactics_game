using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Map/Battle")]
public class Battle : BaseScriptableObject
{
    public BattleType BattleType;
    public Hero Opponent;
    public List<BattleWave> Waves;

    public bool Won;


}