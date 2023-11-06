using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleBossManager : MonoBehaviour
{
    GameManager _gameManager;
    BattleManager _battleManager;
    BattleAreaManager _battleAreaManager;

    [SerializeField] GameObject bossTile;

    void Start()
    {
        _gameManager = GameManager.Instance;
        _battleManager = BattleManager.Instance;
        _battleAreaManager = _battleManager.GetComponent<BattleAreaManager>();

        _battleAreaManager.OnTilePurchased += HandleTilePurchased;
    }

    void HandleTilePurchased()
    {
        // next one is boss
        Debug.Log($"tiles until boss {_battleManager.CurrentBattle.TilesUntilBoss}");
        Debug.Log($"purchased count {_battleAreaManager.PurchasedTiles.Count}");
        if (_battleManager.CurrentBattle.TilesUntilBoss > _battleAreaManager.PurchasedTiles.Count) return;
        _battleAreaManager.OnTilePurchased -= HandleTilePurchased;

        Debug.Log($" boss battle preparation");
        ReplaceTiles();
    }

    void ReplaceTiles()
    {
        // for now it happens when player starts last battle, before they can purchase any tiles
        // if I ever want to create a tile that makes boss tile appear later, I'll need to change this

        foreach (BattleTile tile in _battleAreaManager.PurchasedTiles)
        {
            List<BattleTile> adjacentTiles = new(_battleAreaManager.GetAdjacentTiles(tile));
            foreach (BattleTile adjacentTile in adjacentTiles)
            {
                if (adjacentTile.gameObject.activeSelf) continue;

                _battleAreaManager.ReplaceTile(adjacentTile, bossTile);
            }
        }
    }

}
