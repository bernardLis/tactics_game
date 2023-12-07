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

    List<BattleBreakableVase> _vases = new();

    int _vasesPerSpawn = 5;

    void Start()
    {
        _battleManager = BattleManager.Instance;
        _battleAreaManager = GetComponent<BattleAreaManager>();
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
        BattleBreakableVase vase = Instantiate(_vasePrefab, position, Quaternion.identity);
        _vases.Add(vase);
        vase.OnBroken += () => _vases.Remove(vase);
        vase.transform.localScale = Vector3.zero;
        vase.transform.DOScale(2, 0.5f).SetEase(Ease.OutBack);
        vase.transform.parent = _battleManager.EntityHolder;
    }

    public void BreakAllVases()
    {
        for (int i = _vases.Count - 1; i >= 0; i--)
            _vases[i].TriggerBreak();
    }

}
