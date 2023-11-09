using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class BattleBoss : BattleEntity
{
    BattleAreaManager _battleAreaManager;

    [Header("Boss")]
    [SerializeField] GameObject _buildingCorruptionEffectPrefab;
    GameObject _buildingCorruptionEffect;

    List<BattleTile> _pathToHomeTile = new();
    BattleTile _currentTile;
    BattleBuilding _currentBuilding;

    IEnumerator _corruptionCoroutine;

    public override void InitializeBattle(ref List<BattleEntity> opponents)
    {
        base.InitializeBattle(ref opponents);

        _battleAreaManager = _battleManager.GetComponent<BattleAreaManager>();
        _pathToHomeTile = _battleAreaManager.GetTilePathFromTo(
                                 _battleAreaManager.GetTileFromPosition(transform.position),
                                 _battleAreaManager.HomeTile);


        // HERE: testing - boss
        for (int i = 0; i < _pathToHomeTile.Count; i++)
        {
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

        for (int i = 0; i < _pathToHomeTile.Count; i++)
        {
            // first tile is where boss is spawned
            if (i == 0)
            {
                yield return new WaitForSeconds(2f);
                continue;
            }
            _currentTile = _pathToHomeTile[i];
            _currentBuilding = _currentTile.BattleBuilding;

            yield return PathToPositionAndStop(_currentBuilding.transform.position);

            _corruptionCoroutine = BuildingCorruptionCoroutine();
            yield return _corruptionCoroutine;

            yield return new WaitForSeconds(5f);
        }
        Debug.Log($"boss: end of path ");
    }

    IEnumerator BuildingCorruptionCoroutine()
    {
        Vector3 pos = _currentBuilding.transform.position;
        pos.y = 0.02f;
        _buildingCorruptionEffect = Instantiate(_buildingCorruptionEffectPrefab, pos, Quaternion.identity);
        _buildingCorruptionEffect.transform.localScale = Vector3.zero;
        float scale = _currentBuilding.transform.localScale.x;
        yield return _buildingCorruptionEffect.transform.DOScale(scale, 0.5f)
                                              .SetEase(Ease.OutBack);

        _buildingCorruptionEffect.transform.DORotate(new Vector3(0, 360, 0), 10f, RotateMode.FastBeyond360)
                                           .SetRelative(true)
                                           .SetLoops(-1, LoopType.Incremental)
                                           .SetEase(Ease.InOutSine);

        for (int i = 0; i < 10; i++)
        {
            Debug.Log($"building corrupted in {10 - i} seconds");
            yield return new WaitForSeconds(1);
        }

        _buildingCorruptionEffect.transform.DOKill();
        _buildingCorruptionEffect.transform.DOScale(0, 1f)
                                           .SetEase(Ease.InBack)
                                           .OnComplete(() => Destroy(_buildingCorruptionEffect));

        _currentBuilding.Corrupted();
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
