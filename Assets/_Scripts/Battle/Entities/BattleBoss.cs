using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BattleBoss : BattleEntity
{
    BattleAreaManager _battleAreaManager;
    List<BattleTile> _pathToHomeTile = new();
    // on start get path to home tile
    // move from tile to tile on the path until home tile reached
    public override void InitializeBattle(ref List<BattleEntity> opponents)
    {
        base.InitializeBattle(ref opponents);

        _battleAreaManager = _battleManager.GetComponent<BattleAreaManager>();
        // get path to home tile
        _pathToHomeTile = _battleAreaManager.GetTilePathFromTo(
                                 _battleAreaManager.GetTileFromPosition(transform.position),
                                 _battleAreaManager.HomeTile);


        for (int i = 0; i < _pathToHomeTile.Count; i++)
        {
            Debug.Log($"path to home tile {i}: {_pathToHomeTile[i].transform.position}");
            Debug.DrawLine(_pathToHomeTile[i].transform.position, _pathToHomeTile[i].transform.position + Vector3.up * 10f, Color.red, 100f);

            GameObject newGo = new GameObject();
            newGo.transform.position = _pathToHomeTile[i].transform.position;
            newGo.name = $"{i}";
        }

        StartRunEntityCoroutine();
    }


    // public override void StopRunEntityCoroutine()
    // {
    //     // override to do nothing
    // }

    protected override IEnumerator RunEntity()
    {
        Debug.Log($"run entity");
        _avoidancePriorityRange = new Vector2Int(0, 1);
        // move from tile to tile on the path until home tile reached
        for (int i = 0; i < _pathToHomeTile.Count; i++)
        {
            Debug.Log($"move to tile {i}");
            yield return PathToPositionAndStop(_pathToHomeTile[i].transform.position);
            yield return new WaitForSeconds(10f);
        }
        Debug.Log($"boss: end of path ");
    }

    public override void GetEngaged(BattleEntity engager)
    {
        // boss is never engaged
    }

    public override void BaseGetHit(int dmg, Color color, EntityFight attacker = null)
    {
        EntityLog.Add($"{_battleManager.GetTime()}: Entity takes damage {dmg}");

        if (_getHitSound != null) _audioManager.PlaySFX(_getHitSound, transform.position);
        else _audioManager.PlaySFX("Hit", transform.position);

        DisplayFloatingText(dmg.ToString(), color);

        // OnDamageTaken?.Invoke(dmg);

        int d = Mathf.Clamp(dmg, 0, Entity.CurrentHealth.Value);
        Entity.CurrentHealth.ApplyChange(-d);
        if (Entity.CurrentHealth.Value <= 0)
        {
            TriggerDieCoroutine(attacker, true);
            return;
        }
    }

}
