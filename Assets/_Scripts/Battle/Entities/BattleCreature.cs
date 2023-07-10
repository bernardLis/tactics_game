using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;

public class BattleCreature : BattleEntity
{

    [SerializeField] protected Sound _attackSound;
    [SerializeField] protected Sound _specialAbilitySound;

    public Creature Creature { get; private set; }

    List<BattleEntity> _opponentList = new();

    public BattleEntity Opponent { get; private set; }

    protected float _currentAttackCooldown;
    public float CurrentSpecialAbilityCooldown { get; private set; }

    public int KilledEnemiesCount { get; private set; }
    public int DamageDealt { get; private set; }

    protected bool _hasSpecialAction; // e.g. Shell's shield, can be fired at "any time"
    protected bool _hasSpecialMove;
    protected bool _hasSpecialAttack;
    IEnumerator _attackCoroutine;

    public event Action<int> OnEnemyKilled;
    public event Action<int> OnDamageDealt;

    protected virtual void Start()
    {
    }
    protected virtual void Update()
    {
        if (_currentAttackCooldown >= 0)
            _currentAttackCooldown -= Time.deltaTime;
        if (CurrentSpecialAbilityCooldown >= 0)
            CurrentSpecialAbilityCooldown -= Time.deltaTime;
    }

    public override void InitializeEntity(Entity entity)
    {
        base.InitializeEntity(entity);
        Creature = (Creature)entity;

        OnEnemyKilled += Creature.AddKill;
        OnDamageDealt += Creature.AddDmgDealt;
        OnDamageTaken += Creature.AddDmgTaken;

        _agent.stoppingDistance = Creature.AttackRange;
    }

    public override void InitializeBattle(int team, ref List<BattleEntity> opponents)
    {
        base.InitializeBattle(team, ref opponents);
        _opponentList = opponents;

        _currentAttackCooldown = 0;
        CurrentSpecialAbilityCooldown = 0;

        StartRunEntityCoroutine();
    }

    public override void StopRunEntityCoroutine()
    {
        base.StopRunEntityCoroutine();
        if (_attackCoroutine != null)
            StopCoroutine(_attackCoroutine);
    }

    protected override IEnumerator RunEntity()
    {
        if (IsDead) yield break;

        while (_opponentList.Count == 0)
        {
            if (IsDead) yield break;
            if (_battleManager.BattleFinalized)
            {
                yield return Celebrate();
                yield break;
            }
            yield return new WaitForSeconds(0.5f); // TODO: lag
        }

        if (_hasSpecialAction && CurrentSpecialAbilityCooldown <= 0)
            yield return SpecialAbility();

        if (Opponent == null || Opponent.IsDead)
            ChooseNewTarget();
        yield return new WaitForSeconds(0.1f);

        yield return PathToTarget();

        _attackCoroutine = Attack();
        yield return _attackCoroutine;
    }

    protected override IEnumerator PathToTarget()
    {
        EntityLog.Add($"{Time.time}: Path to target is called");

        if (_hasSpecialMove && CurrentSpecialAbilityCooldown <= 0)
        {
            yield return SpecialAbility();
            PathToTarget();
            yield break;
        }

        _agent.enabled = true;
        _agent.avoidancePriority = Random.Range(1, 100);

        while (!_agent.SetDestination(Opponent.transform.position)) yield return null;
        Animator.SetBool("Move", true);
        while (_agent.pathPending) yield return null;

        // path to target
        while (_agent.enabled && _agent.remainingDistance > _agent.stoppingDistance)
        {
            if (_hasSpecialAction && CurrentSpecialAbilityCooldown <= 0)
                yield return SpecialAbility();

            if (Opponent.IsDead || Opponent == null) yield break;

            _agent.SetDestination(Opponent.transform.position);
            yield return null;
        }

        // reached destination
        _agent.avoidancePriority = 0;
        Animator.SetBool("Move", false);
        _agent.enabled = false;
    }

    protected virtual IEnumerator Attack()
    {
        EntityLog.Add($"{Time.time}: Entity attacked {Opponent.name}");

        // meant to be overwritten

        // it goes at the end... is that a good idea?
        _attackCoroutine = null;
        _currentAttackCooldown = Creature.AttackCooldown;
        StartRunEntityCoroutine();

        yield break;
    }

    protected virtual IEnumerator SpecialAbility()
    {
        EntityLog.Add($"{Time.time}: Entity used special ability");

        // meant to be overwritten

        // it goes at the end... is that a good idea?
        Creature.CreatureAbility.Used();
        CurrentSpecialAbilityCooldown = Creature.CreatureAbility.Cooldown;
        yield break;
    }

    protected bool CanAttack()
    {
        return _currentAttackCooldown < 0;
    }

    protected bool IsOpponentInRange()
    {
        if (Opponent == null) return false;
        if (Opponent.IsDead) return false;

        // +0.5 wiggle room
        return Vector3.Distance(transform.position, Opponent.transform.position) < Creature.AttackRange + 0.5f;
    }


    protected void ChooseNewTarget()
    {
        // choose a random opponent with a bias towards closer opponents
        Dictionary<BattleEntity, float> distances = new();
        foreach (BattleEntity be in _opponentList)
        {
            if (be.IsDead) continue;
            float distance = Vector3.Distance(transform.position, be.transform.position);
            distances.Add(be, distance);
        }

        if (distances.Count == 0) return;

        var closest = distances.OrderByDescending(pair => pair.Value).Reverse().Take(10);
        float v = Random.value;

        Dictionary<BattleEntity, float> closestBiased = new();

        // this number decides bias towards closer opponents
        float sum = 0;
        foreach (KeyValuePair<BattleEntity, float> entry in closest)
        {
            float value = 1 / entry.Value; // 2 / entry.value or 0.1 / entry.value to changed bias
            closestBiased.Add(entry.Key, value);
            sum += value;
        }

        Dictionary<BattleEntity, float> closestNormalized = new();
        foreach (KeyValuePair<BattleEntity, float> entry in closestBiased)
            closestNormalized.Add(entry.Key, entry.Value / sum);

        foreach (KeyValuePair<BattleEntity, float> entry in closestNormalized)
        {
            if (v < entry.Value)
            {
                SetOpponent(entry.Key);
                return;
            }
            v -= entry.Value;
        }

        // should never get here...
        SetOpponent(_opponentList[Random.Range(0, _opponentList.Count)]);
    }

    public void SetOpponent(BattleEntity opponent) { Opponent = opponent; }

    public override void Grabbed()
    {
        base.Grabbed();
        Opponent = null;
    }

    public void DealtDamage(int dmg)
    {
        DamageDealt += dmg;
        OnDamageDealt?.Invoke(dmg);
    }

    public void IncreaseKillCount()
    {
        KilledEnemiesCount++;
        OnEnemyKilled?.Invoke(KilledEnemiesCount);
    }
#if UNITY_EDITOR
    [ContextMenu("Trigger Special Ability")]
    public void TriggerSpecialAction()
    {
        StartCoroutine(SpecialAbility());

    }
#endif

}
