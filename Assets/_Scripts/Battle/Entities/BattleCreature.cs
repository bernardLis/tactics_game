using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Linq;
using DG.Tweening;

public class BattleCreature : BattleEntity
{
    [SerializeField] protected Sound _attackSound;

    [SerializeField] GameObject _gravePrefab;

    public Creature Creature { get; private set; }

    List<BattleEntity> _opponentList = new();

    public BattleEntity Opponent { get; private set; }

    protected float _currentAttackCooldown;
    public float CurrentAbilityCooldown { get; private set; }

    public int DamageDealt { get; private set; }

    public event Action OnEnemyKilled;
    public event Action<int> OnDamageDealt;
    public event Action<BattleCreature> OnEvolving;
    protected virtual void Update()
    {
        if (_currentAttackCooldown >= 0)
            _currentAttackCooldown -= Time.deltaTime;
        if (CurrentAbilityCooldown >= 0)
            CurrentAbilityCooldown -= Time.deltaTime;
    }

    public override void InitializeEntity(Entity entity)
    {
        base.InitializeEntity(entity);
        Creature = (Creature)entity;
        Creature.OnLevelUp += OnLevelUp;

        OnEnemyKilled += Creature.AddKill;
        OnDamageDealt += Creature.AddDmgDealt;
        OnDamageTaken += Creature.AddDmgTaken;

        _agent.stoppingDistance = Creature.AttackRange;
        _avoidancePriorityRange = new Vector2Int(0, 20);
    }

    public override void InitializeBattle(int team, ref List<BattleEntity> opponents)
    {
        base.InitializeBattle(team, ref opponents);
        _opponentList = opponents;

        _currentAttackCooldown = 0;
        CurrentAbilityCooldown = 0;

        StartRunEntityCoroutine();

        if (team == 0) return;
        _GFX.GetComponentInChildren<SkinnedMeshRenderer>().material.shader
                = _gameManager.GameDatabase.SepiaToneShader;
    }

    protected override IEnumerator RunEntity()
    {
        while (true)
        {
            if (IsDead) yield break;
            yield return new WaitWhile(() => Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f);
            yield return ManagePathing();
            yield return ManageAttackCoroutine();
        }
    }

    protected virtual IEnumerator ManageCreatureAbility()
    {
        if (!CanUseAbility()) yield break;

        if (_currentAbilityCoroutine != null)
            StopCoroutine(_currentAbilityCoroutine);
        _currentAbilityCoroutine = CreatureAbility();
        yield return _currentAbilityCoroutine;
    }

    protected bool CanUseAbility()
    {
        if (!Creature.CanUseAbility()) return false;
        if (CurrentAbilityCooldown > 0) return false;
        return true;
    }

    protected IEnumerator ManagePathing()
    {
        if (Opponent == null || Opponent.IsDead)
            ChooseNewTarget();
        yield return new WaitForSeconds(0.1f);

        // HERE: creature could be patrolling or sth
        if (Opponent == null)
        {
            StopRunEntityCoroutine();
            if (Team == 0) _battleManager.OnOpponentEntityAdded += OpponentWasAdded;
            if (Team == 1) _battleManager.OnPlayerCreatureAdded += OpponentWasAdded;
            yield break;
        }

        if (_currentSecondaryCoroutine != null)
            StopCoroutine(_currentSecondaryCoroutine);
        _currentSecondaryCoroutine = PathToOpponent();
        yield return _currentSecondaryCoroutine;
    }

    void OpponentWasAdded(BattleEntity _)
    {
        if (this == null) return;
        StartRunEntityCoroutine();
        if (Team == 0) _battleManager.OnOpponentEntityAdded -= OpponentWasAdded;
        if (Team == 1) _battleManager.OnPlayerCreatureAdded -= OpponentWasAdded;
    }

    protected IEnumerator ManageAttackCoroutine()
    {
        if (_currentSecondaryCoroutine != null)
            StopCoroutine(_currentSecondaryCoroutine);
        _currentSecondaryCoroutine = Attack();
        yield return _currentSecondaryCoroutine;
    }


    protected virtual IEnumerator PathToOpponent()
    {
        yield return PathToPosition(Opponent.transform.position);

        _agent.stoppingDistance = Creature.AttackRange;
        while (_agent.enabled && _agent.remainingDistance > _agent.stoppingDistance)
        {
            _agent.SetDestination(Opponent.transform.position);
            yield return new WaitForSeconds(0.1f);
        }

        // reached destination
        _agent.avoidancePriority = 0;
        Animator.SetBool("Move", false);
        _agent.enabled = false;
    }

    public override void Engage(BattleEntity engager)
    {
        if (_isEngaged) return;
        _isEngaged = true;

        EntityLog.Add($"{_battleManager.GetTime()}: Creature gets engaged by {engager.name}");
        Opponent = engager;
        StartRunEntityCoroutine();
    }

    protected virtual IEnumerator Attack()
    {
        EntityLog.Add($"{_battleManager.GetTime()}: Entity attacked {Opponent.name}");

        while (!CanAttack()) yield return null;
        if (!IsOpponentInRange()) yield break;
        _currentAttackCooldown = Creature.AttackCooldown;

        if (_attackSound != null) _audioManager.PlaySFX(_attackSound, transform.position);
        yield return transform.DODynamicLookAt(Opponent.transform.position, 0.2f, AxisConstraint.Y);
        Animator.SetTrigger("Attack");

        yield return new WaitWhile(() => Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.7f);
    }

    protected virtual IEnumerator CreatureAbility()
    {
        EntityLog.Add($"{_battleManager.GetTime()}: Entity uses ability");

        Creature.CreatureAbility.Used();
        CurrentAbilityCooldown = Creature.CreatureAbility.Cooldown;

        Animator.SetTrigger("Creature Ability");
        yield return new WaitWhile(() => Animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.7f);
        if (Creature.CreatureAbility.Sound != null)
            _audioManager.PlaySFX(Creature.CreatureAbility.Sound, transform.position);
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
        if (_opponentList.Count == 0)
        {
            Opponent = null;
            return;
        }

        // choose a random opponent with a bias towards closer opponents
        Dictionary<BattleEntity, float> distances = new();
        foreach (BattleEntity be in _opponentList)
        {
            if (be.IsDead) continue;
            float distance = Vector3.Distance(transform.position, be.transform.position);
            distances.Add(be, distance);
        }

        var closest = distances.OrderByDescending(pair => pair.Value).Reverse().Take(10);
        float v = Random.value;

        Dictionary<BattleEntity, float> closestBiased = new();

        float sum = 0;
        foreach (KeyValuePair<BattleEntity, float> entry in closest)
        {
            // this number decides bias towards closer opponents
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
                EntityLog.Add($"{_battleManager.GetTime()}: Choosing {entry.Key.name} as new target, #{closestNormalized.Values.ToList().IndexOf(entry.Value)} closest.");
                SetOpponent(entry.Key);
                return;
            }
            v -= entry.Value;
        }
    }

    public void SetOpponent(BattleEntity opponent)
    {
        Opponent = opponent;
        Opponent.OnDeath += (_, _) =>
        {
            if (this == null) return;
            StartRunEntityCoroutine();
        };
    }

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
        OnEnemyKilled?.Invoke();
    }

    public override IEnumerator Die(GameObject attacker = null, bool hasLoot = true, bool hasGrave = true)
    {
        yield return base.Die(attacker, hasLoot);

        _battleManager.OnOpponentEntityAdded -= OpponentWasAdded;

        if (Team != 0) yield break;
        if (!hasGrave) yield break;
        GameObject g = Instantiate(_gravePrefab, transform.position, Quaternion.identity);
        g.GetComponent<BattleCreatureGrave>().Initialize(Creature);
    }

    void OnLevelUp()
    {
        DisplayFloatingText("Level Up!", Color.white);
        CurrentHealth.SetValue(Creature.GetMaxHealth());
    }

    public virtual void Evolve()
    {
        SetDead();
        _blockRunEntity = true;
        _battleEntityHighlight.DisableHighlightFully();

        StopRunEntityCoroutine();
        OnEvolving?.Invoke(this);
    }

#if UNITY_EDITOR
    [ContextMenu("Level up")]
    public void LevelUp()
    {
        Creature.LevelUp();
    }

    [ContextMenu("Trigger Ability")]
    public void TriggerAbility()
    {
        StartCoroutine(CreatureAbility());
    }


    [ContextMenu("Trigger Death")]
    public void TriggerDeath()
    {
        TriggerDieCoroutine(hasGrave: true);
    }

#endif

}
