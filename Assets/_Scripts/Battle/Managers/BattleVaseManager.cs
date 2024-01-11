using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class BattleVaseManager : PoolManager<BattleBreakableVase>
{
    BattleInputManager _battleInputManager;
    BattleAreaManager _battleAreaManager;

    [SerializeField] BattleBreakableVase _vasePrefab;

    int _vasesPerSpawn = 5;

    [SerializeField] bool _debugSpawnVase;

    public void Initialize()
    {
        _battleAreaManager = GetComponent<BattleAreaManager>();
        _battleInputManager = GetComponent<BattleInputManager>();

#if UNITY_EDITOR
        _battleInputManager.OnLeftMouseClick += DebugSpawnVase;
#endif
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

    void DebugSpawnVase()
    {
        if (!_debugSpawnVase) return;

        Mouse mouse = Mouse.current;
        Vector3 mousePosition = mouse.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        int layerMask = Tags.BattleFloorLayer;
        if (Physics.Raycast(ray, out RaycastHit hit, 1000, layerMask))
            SpawnVase(hit.point);
    }
}
