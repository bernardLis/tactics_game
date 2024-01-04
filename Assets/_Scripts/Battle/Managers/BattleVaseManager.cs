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

    List<BattleBreakableVase> Vases = new();

    int _vasesPerSpawn = 5;

    void Start()
    {
        _battleManager = BattleManager.Instance;
        _battleAreaManager = GetComponent<BattleAreaManager>();
        CreateVasePool();
        StartCoroutine(SpawnVasesCoroutine());
    }

    void CreateVasePool()
    {
        GameObject vaseHolder = new("Vase Holder");
        Vases = new();

        for (int i = 0; i < 200; i++)
        {
            BattleBreakableVase v = Instantiate(_vasePrefab, vaseHolder.transform);
            v.gameObject.SetActive(false);
            Vases.Add(v);
        }
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
        BattleBreakableVase vase = Vases.Find(v => !v.gameObject.activeSelf);
        vase.Initialize(position);

        // BattleBreakableVase vase = Instantiate(_vasePrefab, position, Quaternion.identity);
        // Vases.Add(vase);
        // vase.OnBroken += () => Vases.Remove(vase);
        // vase.transform.position = position;
        // vase.transform.localScale = Vector3.zero;
        // vase.transform.DOScale(2, 0.5f).SetEase(Ease.OutBack);
    }

    public void BreakAllVases()
    {
        foreach (BattleBreakableVase vase in Vases)
            if (vase.gameObject.activeSelf)
                vase.TriggerBreak();
        // for (int i = Vases.Count - 1; i >= 0; i--)
        //     Vases[i].TriggerBreak();
    }

}
