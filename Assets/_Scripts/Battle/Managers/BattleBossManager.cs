using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleBossManager : MonoBehaviour
{
    GameManager _gameManager;
    BattleManager _battleManager;
    BattleAreaManager _battleAreaManager;

    [SerializeField] Building[] _bossBuildings;
    [SerializeField] GameObject _bossTileIndicatorPrefab;
    BattleTile _bossTile;
    GameObject _bossTileIndicator;

    Building _chosenBossBuilding;

    public void Initialize()
    {
        _gameManager = GameManager.Instance;
        _battleManager = BattleManager.Instance;
        _battleAreaManager = _battleManager.GetComponent<BattleAreaManager>();
        _battleAreaManager.OnBossTileUnlocked += SpawnBoss;

        _chosenBossBuilding = _bossBuildings[Random.Range(0, _bossBuildings.Length)];
    }

    void SpawnBoss(BattleTile tile)
    {
        StartCoroutine(SpawnBossCoroutine(tile));
    }


    IEnumerator SpawnBossCoroutine(BattleTile tile)
    {
        Debug.Log($"spawning boss");
        // BattleTile tileToReplace = _battleAreaManager.CornerTiles[Random.Range(0, _battleAreaManager.CornerTiles.Count)];
        _bossTile = _battleAreaManager.ReplaceTile(tile, _chosenBossBuilding);

        yield return new WaitForSeconds(1f);
        _battleAreaManager.UnlockTile(_bossTile);
        yield return new WaitForSeconds(1f);
        _bossTile.Secured();

        // List<BattleTile> tiles = _battleAreaManager.GetTilePathFromTo(_bossTile, _battleAreaManager.HomeTile);
        // // I could pass it on to the boss

        // foreach (BattleTile t in tiles)
        // {
        //     if (t.gameObject.activeSelf) continue;
        //     _battleAreaManager.UnlockTile(t);
        // }

    }

}
