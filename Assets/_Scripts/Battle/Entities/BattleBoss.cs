using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using Random = UnityEngine.Random;

public class BattleBoss : BattleEntity
{
    BattleAreaManager _battleAreaManager;

    [Header("Boss")]
    [SerializeField] GameObject _corruptionBreakNodePrefab;
    ProgressBarHandler _stunProgressBar;

    List<BattleTile> _pathToHomeTile = new();
    int _nextTileIndex;
    BattleTile _currentTile;
    BattleBuilding _currentBuilding;

    bool _isCorrupting;
    bool _isStunned;
    int _totalDamageToBreakCorruption = 1000;
    int _currentDamageToBreakCorruption;

    List<BattleCorruptionBreakNode> _corruptionBreakNodes = new();

    public event Action OnCorruptionBroken;

    public override void InitializeBattle(ref List<BattleEntity> opponents)
    {
        base.InitializeBattle(ref opponents);

        _stunProgressBar = GetComponentInChildren<ProgressBarHandler>();
        _stunProgressBar.Initialize();

        _battleAreaManager = _battleManager.GetComponent<BattleAreaManager>();
        _pathToHomeTile = _battleAreaManager.GetTilePathFromTo(
                                 _battleAreaManager.GetTileFromPosition(transform.position),
                                 _battleAreaManager.HomeTile);

        _nextTileIndex = 0;
        _battleManager.GetComponent<BattleTooltipManager>().ShowBossHealthBar(this);
        StartRunEntityCoroutine();
    }

    protected override IEnumerator RunEntity()
    {
        Debug.Log($"run entity");
        _avoidancePriorityRange = new Vector2Int(0, 1);

        for (int i = _nextTileIndex; i < _pathToHomeTile.Count; i++)
        {
            // first tile is where the boss is spawned
            if (i == 0)
            {
                yield return new WaitForSeconds(2f);
                continue;
            }
            _nextTileIndex = i + 1;
            _currentTile = _pathToHomeTile[i];
            _currentBuilding = _pathToHomeTile[i].BattleBuilding;

            yield return PathToPositionAndStop(_currentBuilding.transform.position);

            StartBuildingCorruption();
            yield return new WaitForSeconds(1f);
        }
    }

    /* CORRUPTION */

    void StartBuildingCorruption()
    {
        Animator.SetTrigger("Creature Ability");

        StopRunEntityCoroutine();
        _isCorrupting = true;
        _stunProgressBar.ShowProgressBar();
        _currentDamageToBreakCorruption = _totalDamageToBreakCorruption;

        _currentBuilding.GetCorrupted(this);
        _currentBuilding.OnBuildingCorrupted += OnBuildingCorrupted;
        StartCoroutine(CreateCorruptionBreakNodes());
    }

    void OnBuildingCorrupted()
    {
        CorruptionCleanup();
        StartRunEntityCoroutine();
    }

    IEnumerator CreateCorruptionBreakNodes()
    {
        _corruptionBreakNodes = new();
        for (int i = 0; i < 3; i++)
        {
            if (!_isCorrupting) yield break;
            BattleCorruptionBreakNode node = Instantiate(_corruptionBreakNodePrefab,
                                            transform.position, Quaternion.identity)
                                            .GetComponent<BattleCorruptionBreakNode>();
            node.Initialize(this, _currentTile.GetPositionRandom(0, 0));
            node.OnNodeBroken += OnCorruptionNodeBroken;
            _corruptionBreakNodes.Add(node);
            yield return new WaitForSeconds(Random.Range(1f, 3f));
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

    void HandleCorruptionBreak(int damage)
    {
        _currentDamageToBreakCorruption -= damage;
        _stunProgressBar.SetProgress(1f - (float)_currentDamageToBreakCorruption / _totalDamageToBreakCorruption);
        // HERE: show stun progress bar

        if (_currentDamageToBreakCorruption > 0) return;

        CorruptionCleanup();
        OnCorruptionBroken?.Invoke();
        StartCoroutine(StunCoroutine());
    }

    void CorruptionCleanup()
    {
        if (!_isStunned) _stunProgressBar.HideProgressBar();
        _isCorrupting = false;
        _currentBuilding.OnBuildingCorrupted -= OnBuildingCorrupted;
        DestroyAllCorruptionBreakNodes();
    }

    IEnumerator StunCoroutine()
    {
        DisplayFloatingText("Stunned", Color.yellow);
        // HERE: use stun progress bar to display stun duration

        _isStunned = true;
        StopRunEntityCoroutine();
        for (int i = 0; i < 10; i++)
        {
            _stunProgressBar.SetProgress((10 - i) * 0.1f);
            yield return new WaitForSeconds(1f);
        }
        _stunProgressBar.HideProgressBar();
        StartRunEntityCoroutine();
        _isStunned = false;
    }

    /* GET HIT */
    public override void GetEngaged(BattleEntity engager)
    {
        // boss is never engaged
    }

    public override void BaseGetHit(int dmg, Color color, EntityFight attacker = null)
    {
        if (_isStunned) dmg *= 2;

        EntityLog.Add($"{_battleManager.GetTime()}: Entity takes damage {dmg}");

        if (_getHitSound != null) _audioManager.PlaySFX(_getHitSound, transform.position);
        else _audioManager.PlaySFX("Hit", transform.position);

        if (_isCorrupting) color = Color.yellow; // signifying stun
        DisplayFloatingText(dmg.ToString(), color);

        int d = Mathf.Clamp(dmg, 0, Entity.CurrentHealth.Value);
        Entity.CurrentHealth.ApplyChange(-d);
        if (Entity.CurrentHealth.Value <= 0)
        {
            TriggerDieCoroutine(attacker, true);
            return;
        }

        if (_isCorrupting) HandleCorruptionBreak(d);
    }

}
