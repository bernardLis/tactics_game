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

    [Header("Shooting")]
    [SerializeField] GameObject _projectilePrefab;
    IEnumerator _shootingCoroutine;
    int _currentShottingPatternIndex;
    public List<BattleProjectileBoss> _projectilePool = new();

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
        StartRunEntityCoroutine();
        StartShootingCoroutine();

        TotalDamageToBreakCorruption = ScriptableObject.CreateInstance<IntVariable>();
        TotalDamageToBreakCorruption.SetValue(1000);
        CurrentDamageToBreakCorruption = ScriptableObject.CreateInstance<IntVariable>();
        CurrentDamageToBreakCorruption.SetValue(0);
        TotalStunDuration = ScriptableObject.CreateInstance<IntVariable>();
        TotalStunDuration.SetValue(10);
        CurrentStunDuration = ScriptableObject.CreateInstance<IntVariable>();
        CurrentStunDuration.SetValue(0);

        _battleManager.GetComponent<BattleTooltipManager>().ShowBossHealthBar(this);

        _projectilePool = new();
        CreateProjectilePool();
    }

    void CreateProjectilePool()
    {
        for (int i = 0; i < 200; i++)
        {
            BattleProjectileBoss p = Instantiate(_projectilePrefab, transform).GetComponent<BattleProjectileBoss>();
            p.gameObject.SetActive(false);
            _projectilePool.Add(p);
        }
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

            yield return PathToPositionAndStop(_currentBuilding.transform.position);

            StartBuildingCorruption();
            yield return new WaitForSeconds(1f);
        }
    }

    void StartShootingCoroutine()
    {
        _shootingCoroutine = ShootingCoroutine();
        StartCoroutine(_shootingCoroutine);
    }

    IEnumerator ShootingCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(7f);
            if (_isStunned) continue;
            Shoot();
        }
    }

    void Shoot()
    {
        int totalCount = Random.Range(20, 50);
        if (_currentShottingPatternIndex == 0)
            ShootInCircle(totalCount);
        if (_currentShottingPatternIndex == 1)
            StartCoroutine(ShootInCircleWithDelay(totalCount));
        if (_currentShottingPatternIndex == 2)
            StartCoroutine(RandomShots(totalCount));
        if (_currentShottingPatternIndex == 3)
            StartCoroutine(RhombusShots(totalCount));

        _currentShottingPatternIndex++;
        if (_currentShottingPatternIndex >= 4) _currentShottingPatternIndex = 0;
    }

    void ShootInCircle(int total)
    {
        for (int i = 0; i < total; i++)
        {
            Vector3 spawnPos = transform.position;
            spawnPos.y = 1f;
            Vector3 pos = GetPositionOnCircle(i, total);
            pos.y = 1f;
            Vector3 dir = (pos - spawnPos).normalized;
            SpawnProjectile(dir, 10f, 5);
        }
    }

    IEnumerator ShootInCircleWithDelay(int total)
    {
        float waitTime = 3f / total;
        for (int i = 0; i < total; i++)
        {
            Vector3 spawnPos = transform.position;
            spawnPos.y = 1f;
            Vector3 pos = GetPositionOnCircle(i, total);
            pos.y = 1f;
            Vector3 dir = (pos - spawnPos).normalized;
            SpawnProjectile(dir, 10f, 5);
            yield return new WaitForSeconds(waitTime);
        }
    }

    IEnumerator RandomShots(int total)
    {
        float waitTime = 3f / total;
        for (int i = 0; i < total; i++)
        {
            Vector3 dir = Quaternion.Euler(0, Random.Range(0, 360), 0) * Vector3.forward;
            SpawnProjectile(dir, 10f, 5);
            yield return new WaitForSeconds(waitTime);
        }
    }

    IEnumerator RhombusShots(int total)
    {
        int numberOfGroups = total / 4;
        float waitTime = 3f / numberOfGroups;
        for (int i = 0; i < numberOfGroups; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                Vector3 dir = Quaternion.Euler(0, i * 15 + j * 90, 0) * Vector3.forward;
                SpawnProjectile(dir, 10f, 5);
            }
            yield return new WaitForSeconds(waitTime);
        }
    }

    void SpawnProjectile(Vector3 dir, float time, int power)
    {
        Vector3 spawnPos = transform.position;
        spawnPos.y = 1f;
        BattleProjectileBoss p = _projectilePool.Find(x => !x.gameObject.activeSelf);
        p.transform.position = spawnPos;
        p.Initialize(1);
        // BattleProjectileBoss p = Instantiate(_projectilePrefab, transform).GetComponent<BattleProjectileBoss>();
        p.Shoot(this, dir, time, power);
    }

    Vector3 GetPositionOnCircle(int currentIndex, int totalCount)
    {
        float theta = currentIndex * 2 * Mathf.PI / totalCount;
        float radius = 5;
        Vector3 center = transform.position;
        float x = Mathf.Cos(theta) * radius + center.x;
        float y = 1f;
        float z = Mathf.Sin(theta) * radius + center.z;

        return new(x, y, z);

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
            yield return new WaitForSeconds(Random.Range(1f, 3f));

            if (!_isCorrupting) yield break;
            BattleCorruptionBreakNode node = Instantiate(_corruptionBreakNodePrefab,
                                            transform.position, Quaternion.identity)
                                            .GetComponent<BattleCorruptionBreakNode>();
            Vector3 pos = Vector3.zero;
            while (pos == Vector3.zero)
            {
                pos = _currentTile.GetPositionRandom(0, 0);
                if (Vector3.Distance(pos, transform.position) < 2f) pos = Vector3.zero;
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

        OnCorruptionBroken?.Invoke();
        StartCoroutine(StunCoroutine());

        // can't break corruption on the last building
        if (_nextTileIndex < _pathToHomeTile.Count)
            CorruptionCleanup();
    }

    void CorruptionCleanup()
    {
        _isCorrupting = false;
        _currentBuilding.OnBuildingCorrupted -= OnBuildingCorrupted;
        DestroyAllCorruptionBreakNodes();
    }

    IEnumerator StunCoroutine()
    {
        Debug.Log($"boss Stunned");
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
        Debug.Log($"boss Stunned finished");

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
