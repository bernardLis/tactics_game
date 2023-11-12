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
    [SerializeField] GameObject _stunEffect;

    List<BattleTile> _pathToHomeTile = new();
    int _nextTileIndex;
    BattleTile _currentTile;
    BattleBuilding _currentBuilding;

    bool _isCorrupting;
    bool _isStunned;
    public IntVariable TotalDamageToBreakCorruption;
    public IntVariable CurrentDamageToBreakCorruption;
    public IntVariable TotalStunDuration;
    public IntVariable CurrentStunDuration;

    List<BattleCorruptionBreakNode> _corruptionBreakNodes = new();

    public event Action OnCorruptionStarted;
    public event Action OnCorruptionBroken;

    public override void InitializeBattle(ref List<BattleEntity> opponents)
    {
        base.InitializeBattle(ref opponents);

        _battleAreaManager = _battleManager.GetComponent<BattleAreaManager>();
        _pathToHomeTile = _battleAreaManager.GetTilePathFromTo(
                                 _battleAreaManager.GetTileFromPosition(transform.position),
                                 _battleAreaManager.HomeTile);

        _nextTileIndex = 0;
        StartRunEntityCoroutine();

        TotalDamageToBreakCorruption = ScriptableObject.CreateInstance<IntVariable>();
        TotalDamageToBreakCorruption.SetValue(1000);
        CurrentDamageToBreakCorruption = ScriptableObject.CreateInstance<IntVariable>();
        CurrentDamageToBreakCorruption.SetValue(0);
        TotalStunDuration = ScriptableObject.CreateInstance<IntVariable>();
        TotalStunDuration.SetValue(10);
        CurrentStunDuration = ScriptableObject.CreateInstance<IntVariable>();
        CurrentStunDuration.SetValue(0);

        _battleManager.GetComponent<BattleTooltipManager>().ShowBossHealthBar(this);
    }

    protected override IEnumerator RunEntity()
    {
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
        CurrentDamageToBreakCorruption.SetValue(0);

        _currentBuilding.StartCorruption(this);
        _currentBuilding.OnBuildingCorrupted += OnBuildingCorrupted;
        StartCoroutine(CreateCorruptionBreakNodes());
        OnCorruptionStarted?.Invoke();
    }

    void OnBuildingCorrupted()
    {
        if (_isStunned) return;
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
        BaseGetHit(Mathf.RoundToInt(TotalDamageToBreakCorruption.Value * 0.3f), Color.yellow);
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
        CurrentDamageToBreakCorruption.ApplyChange(damage);

        if (CurrentDamageToBreakCorruption.Value < TotalDamageToBreakCorruption.Value) return;

        CorruptionCleanup();
        OnCorruptionBroken?.Invoke();
        StartCoroutine(StunCoroutine());
    }

    void CorruptionCleanup()
    {
        _isCorrupting = false;
        _currentBuilding.OnBuildingCorrupted -= OnBuildingCorrupted;
        DestroyAllCorruptionBreakNodes();
    }

    IEnumerator StunCoroutine()
    {
        DisplayFloatingText("Stunned", Color.yellow);
        CurrentStunDuration.SetValue(TotalStunDuration.Value);

        _stunEffect.SetActive(true);
        _isStunned = true;
        Animator.enabled = false;
        StopRunEntityCoroutine();
        for (int i = 0; i < TotalStunDuration.Value; i++)
        {
            yield return new WaitForSeconds(1f);
            CurrentStunDuration.ApplyChange(-1);
        }
        _stunEffect.SetActive(false);
        Animator.enabled = true;
        StartRunEntityCoroutine();
        _isStunned = false;
    }

    /* GET HIT */
    public override void GetEngaged(BattleEntity engager)
    {
        // boss is never engaged
        // all the single bosses, all the single bosses... 
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
