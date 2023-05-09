using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleLogManager : MonoBehaviour
{
    public List<BattleLog> Logs = new();

    public void Initialize(List<BattleEntity> playerEntities, List<BattleEntity> opponentEntities)
    {
        foreach (BattleEntity entity in playerEntities)
            entity.OnDeath += OnEntityDeath;
        foreach (BattleEntity entity in opponentEntities)
            entity.OnDeath += OnEntityDeath;
    }

    void OnEntityDeath(BattleEntity deadEntity, BattleEntity attacker, Ability ability)
    {
        BattleLogEntityDeath log = ScriptableObject.CreateInstance<BattleLogEntityDeath>();
        log.Initialize(deadEntity, attacker, ability);
        AddLog(log);
    }

    public void AddLog(BattleLog log) { Logs.Add(log); }

}
