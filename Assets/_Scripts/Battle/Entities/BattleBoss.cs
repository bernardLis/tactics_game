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

    [Header("Attacks")]
    List<BossAttack> _attacks = new();
    IEnumerator _attackCoroutine;

    List<BattleTile> _pathToHomeTile = new();
    int _nextTileIndex;
    BattleTile _currentTile;
    BattleBuilding _currentBuilding;

    bool _isCorrupting;
    bool _isStunned;
    [HideInInspector] public IntVariable TotalDamageToBreakCorruption;
    [HideInInspector] public IntVariable CurrentDamageToBreakCorruption;
    [HideInInspector] public IntVariable TotalStunDuration;
    [HideInInspector] public IntVariable CurrentStunDuration;

    List<BattleCorruptionBreakNode> _corruptionBreakNodes = new();

    public event Action OnCorruptionStarted;
    public event Action OnCorruptionBroken;
    public event Action OnStunFinished;

    public override void InitializeBattle(ref List<BattleEntity> opponents)
    {
        base.InitializeBattle(ref opponents);

        _battleAreaManager = _battleManager.GetComponent<BattleAreaManager>();
        _pathToHomeTile = _battleAreaManager.GetTilePathFromTo(
                                 _battleAreaManager.GetTileFromPosition(transform.position),
                                 _battleAreaManager.HomeTile);
        _nextTileIndex = 0;

        float newSpeed = _agent.speed - _gameManager.GlobalUpgradeBoard.GetUpgradeByName("Slowdown").GetValue();
        if (newSpeed <= 0) newSpeed = 1f;
        _agent.speed = newSpeed;

        InitializeAttacks();
        StartRunEntityCoroutine();
        StartAttackCoroutine();

        SetUpVariables();
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

            // if already at the last tile, do nothing
            // if (i == _pathToHomeTile.Count - 1) break;

            _nextTileIndex = i + 1;
            _currentTile = _pathToHomeTile[i];
            _currentBuilding = _pathToHomeTile[i].BattleBuilding;

            Vector3 pos = _currentTile.transform.position;
            if (_currentBuilding != null) pos = _currentBuilding.transform.position;
            yield return PathToPositionAndStop(pos);

            if (_currentBuilding != null) StartBuildingCorruption();
            else yield return new WaitForSeconds(10f);
            yield return new WaitForSeconds(1f);
        }
    }

    /* ATTACKS */
    void InitializeAttacks()
    {
        Boss boss = (Boss)Entity;
        foreach (BossAttack original in boss.Attacks)
        {
            BossAttack attack = Instantiate(original);
            attack.Initialize(this);
            _attacks.Add(attack);
        }
    }

    void StartAttackCoroutine()
    {
        _attackCoroutine = AttackCoroutine();
        StartCoroutine(_attackCoroutine);
    }

    IEnumerator AttackCoroutine()
    {
        while (true)
        {
            for (int i = 0; i < _attacks.Count; i++)
            {
                yield return AttackCooldownCoroutine(_attacks[i]);
                int difficulty = 1; // TODO: difficulty
                yield return _attacks[i].BattleBossAttack.Attack(difficulty);
            }
        }
    }

    IEnumerator AttackCooldownCoroutine(BossAttack bossAttack)
    {
        for (int i = 0; i < bossAttack.CooldownSeconds; i++)
        {
            while (_isStunned) yield return new WaitForSeconds(1f);
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
        OnCorruptionStarted?.Invoke();

        if (_gameManager.GlobalUpgradeBoard.GetUpgradeByName("Corruption Break Nodes").CurrentLevel >= 0)
            StartCoroutine(CreateCorruptionBreakNodes());
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
            yield return new WaitForSeconds(Random.Range(1f, 3f));

            if (!_isCorrupting) yield break;
            BattleCorruptionBreakNode node = Instantiate(_corruptionBreakNodePrefab,
                                            transform.position, Quaternion.identity)
                                            .GetComponent<BattleCorruptionBreakNode>();
            Vector3 pos = Vector3.zero;
            while (pos == Vector3.zero)
            {
                pos = _currentTile.GetPositionRandom(0, 0);
                if (Vector3.Distance(pos, transform.position) < 4f) pos = Vector3.zero;
            }

            node.Initialize(this, pos);
            node.OnNodeBroken += OnCorruptionNodeBroken;
            _corruptionBreakNodes.Add(node);

            yield return new WaitForSeconds(Random.Range(1f, 3f));
        }
    }

    void OnCorruptionNodeBroken(BattleCorruptionBreakNode node)
    {
        _corruptionBreakNodes.Remove(node);
        float multiplier = (float)0.3f +
                        _gameManager.GlobalUpgradeBoard.GetUpgradeByName("Corruption Break Nodes").GetValue() / 100f;
        int damage = Mathf.RoundToInt(TotalDamageToBreakCorruption.Value * multiplier);
        BaseGetHit(damage, Color.yellow);
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
        if (_gameManager.GlobalUpgradeBoard.GetUpgradeByName("Stun").CurrentLevel == -1) return;
        
        StartCoroutine(CreateCorruptionBreakNodes());

        CurrentDamageToBreakCorruption.ApplyChange(damage);

        if (CurrentDamageToBreakCorruption.Value < TotalDamageToBreakCorruption.Value) return;

        OnCorruptionBroken?.Invoke();
        StartCoroutine(StunCoroutine());

        // can't break corruption on the last building
        if (_nextTileIndex < _pathToHomeTile.Count)
            CorruptionCleanup();
    }

    void CorruptionCleanup()
    {
        CurrentDamageToBreakCorruption.SetValue(0);
        _isCorrupting = false;
        _currentBuilding.OnBuildingCorrupted -= OnBuildingCorrupted;
        DestroyAllCorruptionBreakNodes();
    }

    IEnumerator StunCoroutine()
    {
        DisplayFloatingText("Stunned", Color.yellow);
        CurrentDamageToBreakCorruption.SetValue(0);
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
        OnStunFinished?.Invoke();
    }

    /* GET HIT */
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
            TriggerDieCoroutine(attacker);
            return;
        }

        if (_isCorrupting) HandleCorruptionBreak(dmg);
    }

    /* HELPERS */
    void SetUpVariables()
    {
        TotalDamageToBreakCorruption = ScriptableObject.CreateInstance<IntVariable>();
        TotalDamageToBreakCorruption.SetValue(1000);
        CurrentDamageToBreakCorruption = ScriptableObject.CreateInstance<IntVariable>();
        CurrentDamageToBreakCorruption.SetValue(0);
        TotalStunDuration = ScriptableObject.CreateInstance<IntVariable>();
        TotalStunDuration.SetValue(10 + _gameManager.GlobalUpgradeBoard.GetUpgradeByName("Stun").GetValue());
        CurrentStunDuration = ScriptableObject.CreateInstance<IntVariable>();
        CurrentStunDuration.SetValue(0);
    }

    /* EMPTY OVERRIDES */

    public override void GetEngaged(BattleEntity engager)
    {
        // boss is never engaged
        // all the single bosses, all the single bosses... 
    }

    public override bool CanBeGrabbed() { return false; }

}
