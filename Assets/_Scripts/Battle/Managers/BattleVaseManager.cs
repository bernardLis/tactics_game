using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class BattleVaseManager : MonoBehaviour
{
    BattleManager _battleManager;
    BattleAreaManager _battleAreaManager;
    [SerializeField] BattleBreakableVase _vasePrefab;

    int _vasesPerTile = 10;

    void Start()
    {
        _battleManager = BattleManager.Instance;
        _battleAreaManager = GetComponent<BattleAreaManager>();
        _battleAreaManager.OnTilePurchased += HandleTilePurchased;
    }

    void HandleTilePurchased(BattleTile tile)
    {
        if (tile == null) return;

        StartCoroutine(SpawnVasesCoroutine(tile));

    }

    IEnumerator SpawnVasesCoroutine(BattleTile tile)
    {
        yield return new WaitForSeconds(2.5f);

        for (int i = 0; i < _vasesPerTile; i++)
        {
            Vector3 pos = tile.GetPositionRandom(default, default);
            SpawnVase(pos);
            yield return new WaitForSeconds(0.15f);
        }

    }

    void SpawnVase(Vector3 position)
    {
        BattleBreakableVase vase = Instantiate(_vasePrefab, position, Quaternion.identity);
        vase.transform.localScale = Vector3.zero;
        vase.transform.DOScale(2, 0.5f).SetEase(Ease.OutBack);
        vase.transform.parent = _battleManager.EntityHolder;
    }

}
