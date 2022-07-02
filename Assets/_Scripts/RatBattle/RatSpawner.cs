using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatSpawner : MonoBehaviour, IUITextDisplayable
{

    RatBattleManger _ratBattleManger;

    void Awake()
    {
        TurnManager.OnBattleStateChanged += TurnManager_OnBattleStateChanged;
    }

    void Start()
    {
        _ratBattleManger = RatBattleManger.Instance;
    }

    void OnDestroy()
    {
        TurnManager.OnBattleStateChanged -= TurnManager_OnBattleStateChanged;
    }

    void TurnManager_OnBattleStateChanged(BattleState state)
    {
        if (TurnManager.BattleState == BattleState.PlayerTurn)
            HandlePlayerTurn();
    }

    void HandlePlayerTurn()
    {
        // get all with tag enemy, if there are less than 4, spawn rats
        GameObject[] rats = GameObject.FindGameObjectsWithTag("Enemy");
        if (rats.Length < 4 && !IsSpawnCovered())
            _ratBattleManger.SpawnRat(transform.position);
    }

    bool IsSpawnCovered()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 0.2f);
        if (cols.Length > 1) // 1 is self
            return true;

        return false;
    }

    public string DisplayText()
    {
        return "Grate. Perhaps that's were the rats come from?";
    }
}
