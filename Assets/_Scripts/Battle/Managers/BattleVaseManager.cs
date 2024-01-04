using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class BattleVaseManager : PoolManager<BattleBreakableVase>
{
    BattleAreaManager _battleAreaManager;

    [SerializeField] BattleBreakableVase _vasePrefab;

    int _vasesPerSpawn = 5;

    public void Initialize()
    {
        _battleAreaManager = GetComponent<BattleAreaManager>();
        CreatePool(_vasePrefab.gameObject);
        StartCoroutine(SpawnVasesCoroutine());
    }

    IEnumerator SpawnVasesCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(10f, 20f));

            for (int i = 0; i < _vasesPerSpawn; i++)
            {
                BattleTile tile = _battleAreaManager.GetRandomUnlockedTile();

                Vector3 pos = tile.GetPositionRandom(default, default);
                SpawnVase(pos);
                yield return new WaitForSeconds(0.15f);
            }
        }
    }

    void SpawnVase(Vector3 position)
    {
        BattleBreakableVase vase = GetObjectFromPool();
        vase.Initialize(position);
    }

    public void BreakAllVases()
    {
        foreach (BattleBreakableVase vase in GetActiveObjects())
            if (vase.gameObject.activeSelf)
                vase.TriggerBreak();
    }

}
