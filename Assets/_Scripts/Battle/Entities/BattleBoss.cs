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

    [SerializeField] GameObject _corruptionBreakNodePrefab;

    List<BattleTile> _pathToHomeTile = new();
    int _nextTileIndex;
    BattleTile _currentTile;
    BattleBuilding _currentBuilding;

    IEnumerator _corruptionCoroutine;
    bool _isCorrupting;
    int _totalDamageToBreakCorruption = 1000;
    int _currentDamageToBreakCorruption;
    List<BattleCorruptionBreakNode> _corruptionBreakNodes = new();


    public override void InitializeBattle(ref List<BattleEntity> opponents)
    {
        base.InitializeBattle(ref opponents);

        _battleAreaManager = _battleManager.GetComponent<BattleAreaManager>();
        _pathToHomeTile = _battleAreaManager.GetTilePathFromTo(
                                 _battleAreaManager.GetTileFromPosition(transform.position),
                                 _battleAreaManager.HomeTile);

        _nextTileIndex = 0;
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

        for (int i = _nextTileIndex; i < _pathToHomeTile.Count; i++)
        {
            // first tile is where boss is spawned
            if (i == 0)
            {
                yield return new WaitForSeconds(2f);
                continue;
            }
            _nextTileIndex = i + 1;
            _currentTile = _pathToHomeTile[i];
            _currentBuilding = _pathToHomeTile[i].BattleBuilding;

            yield return PathToPositionAndStop(_currentBuilding.transform.position);

            _corruptionCoroutine = BuildingCorruptionCoroutine();
            yield return _corruptionCoroutine;

            yield return new WaitForSeconds(5f);
        }
        Debug.Log($"boss: end of path ");
    }

    IEnumerator BuildingCorruptionCoroutine()
    {
        _isCorrupting = true;
        _currentDamageToBreakCorruption = _totalDamageToBreakCorruption;

        yield return DisplayCorruptionEffect();
        CreateCorruptionBreakNodes();

        for (int i = 0; i < 10; i++)
        {
            // HERE: display corruption progress bar

            Debug.Log($"building corrupted in {10 - i} seconds");
            yield return new WaitForSeconds(1);
        }

        yield return HideCorruptionEffect();
        DestroyAllCorruptionBreakNodes();

        _currentBuilding.Corrupted();
        _isCorrupting = false;
    }

    void CreateCorruptionBreakNodes()
    {
        _corruptionBreakNodes = new();
        for (int i = 0; i < 3; i++)
        {
            // place nodes randomly on the tile
            BattleCorruptionBreakNode node = Instantiate(_corruptionBreakNodePrefab, transform.position, Quaternion.identity).GetComponent<BattleCorruptionBreakNode>();
            node.Initialize(this, _currentTile.GetPositionRandom(0, 0));
            node.OnNodeBroken += OnCorruptionNodeBroken;
            _corruptionBreakNodes.Add(node);
        }
    }

    void OnCorruptionNodeBroken(BattleCorruptionBreakNode node)
    {
        _corruptionBreakNodes.Remove(node);
        BaseGetHit(Mathf.RoundToInt(_totalDamageToBreakCorruption * 0.3f), Color.yellow);
    }

    void DestroyAllCorruptionBreakNodes()
    {
        foreach (BattleCorruptionBreakNode node in _corruptionBreakNodes)
        {
            if (node == null) continue;
            node.OnNodeBroken -= OnCorruptionNodeBroken;
            node.DestroySelf();
        }
        _corruptionBreakNodes = new();

    }

    IEnumerator DisplayCorruptionEffect()
    {
        Vector3 pos = _currentBuilding.transform.position;
        pos.y = 0.02f;
        _buildingCorruptionEffect = Instantiate(_buildingCorruptionEffectPrefab, pos, Quaternion.identity);
        _buildingCorruptionEffect.transform.localScale = Vector3.zero;
        float scale = _currentBuilding.transform.localScale.x;
        yield return _buildingCorruptionEffect.transform.DOScale(scale, 0.5f)
                                              .SetEase(Ease.OutBack)
                                              .WaitForCompletion();

        _buildingCorruptionEffect.transform.DORotate(new Vector3(0, 360, 0), 10f, RotateMode.FastBeyond360)
                                           .SetRelative(true)
                                           .SetLoops(-1, LoopType.Incremental)
                                           .SetEase(Ease.InOutSine);
    }

    IEnumerator HideCorruptionEffect()
    {
        _buildingCorruptionEffect.transform.DOKill();
        yield return _buildingCorruptionEffect.transform.DOScale(0, 1f)
                                            .SetEase(Ease.InBack)
                                            .OnComplete(() => Destroy(_buildingCorruptionEffect))
                                            .WaitForCompletion();
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

        if (_isCorrupting) color = Color.yellow; // signifying stun
        DisplayFloatingText(dmg.ToString(), color);

        // OnDamageTaken?.Invoke(dmg);

        int d = Mathf.Clamp(dmg, 0, Entity.CurrentHealth.Value);
        Entity.CurrentHealth.ApplyChange(-d);
        if (Entity.CurrentHealth.Value <= 0)
        {
            TriggerDieCoroutine(attacker, true);
            return;
        }

        if (_isCorrupting) HandleCorruptionBreak(d);
    }

    void HandleCorruptionBreak(int damage)
    {
        _currentDamageToBreakCorruption -= damage;
        Debug.Log($"_dmgToBreakCorruption {_currentDamageToBreakCorruption}");
        // HERE: show stun progress bar

        if (_currentDamageToBreakCorruption > 0) return;

        DestroyAllCorruptionBreakNodes();
        BreakCorruptionCoroutine();
        StartCoroutine(StunCoroutine());
    }

    void BreakCorruptionCoroutine()
    {
        if (_corruptionCoroutine == null) return;

        Debug.Log($"breaking corruiption coroutine");
        StopCoroutine(_corruptionCoroutine);
        _corruptionCoroutine = null;

        _buildingCorruptionEffect.transform.DOKill();
        _buildingCorruptionEffect.transform.DOScale(0, 0.5f)
                                           .SetEase(Ease.OutBack)
                                           .OnComplete(() => Destroy(_buildingCorruptionEffect));

        _isCorrupting = false;
    }


    IEnumerator StunCoroutine()
    {
        DisplayFloatingText("Stunned", Color.yellow);
        // HERE: use stun progress bar to display stun duration

        Debug.Log($"start of stun");
        StopRunEntityCoroutine();
        yield return new WaitForSeconds(10f);
        StartRunEntityCoroutine();
        Debug.Log($"end of stun");

    }



}
