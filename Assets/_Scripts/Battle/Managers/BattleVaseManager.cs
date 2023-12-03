using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BattleVaseManager : MonoBehaviour
{
    BattleManager _battleManager;
    BattleAreaManager _battleAreaManager;
    [SerializeField] BattleBreakableVase _vasePrefab;


    void Start()
    {
        _battleManager = BattleManager.Instance;
        _battleAreaManager = GetComponent<BattleAreaManager>();
        _battleAreaManager.OnTilePurchased += HandleTilePurchased;
    }

    void HandleTilePurchased(BattleTile tile)
    {
        if (tile == null) return;

        for (int i = 0; i < 10; i++)
        {
            Vector3 pos = tile.GetPositionRandom(default, default);
            SpawnVase(pos);
        }
    }
    
    void SpawnVase(Vector3 position)
    {
        BattleBreakableVase vase = Instantiate(_vasePrefab, position, Quaternion.identity);
        vase.transform.parent = _battleManager.EntityHolder;
    }

}
